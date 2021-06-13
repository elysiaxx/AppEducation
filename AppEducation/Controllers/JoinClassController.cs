using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppEducation.Models;
using Microsoft.AspNetCore.Authorization;
using AppEducation.Models.Users;
using AppEducation.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using AppEducation.Models.RoomInfo;

namespace AppEducation.Controllers
{

    [Authorize]
    public class JoinClassController : Controller
    {
        private readonly AppIdentityDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<ConnectionHub> _hubContext;
        private UserManager<AppUser> userManager;
        public JoinClassController(ILogger<HomeController> logger, AppIdentityDbContext context, IHubContext<ConnectionHub> hubContext, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
                
            return View();
        }
        public IActionResult Create()
        {
            IEnumerable<Classes> classes = _context.Classes;
            JoinClassInfor joinClassInfor = new JoinClassInfor();
            joinClassInfor.AvailableClasses = classes.Where(c => c.isActive == true);
            joinClassInfor.AvailableClasses.ToList().ForEach(c =>
            {
                c.User = _context.Users.SingleOrDefault(u => u.Id == c.UserId);
            });
            return View(joinClassInfor);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Teacher")]
        public async Task<IActionResult> Create(JoinClassInfor joinClassInfor)
        {
            if (ModelState.IsValid)
            {
                AppUser currentUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                joinClassInfor.NewClass.User = currentUser;
                
                joinClassInfor.NewClass.isActive = true;
                //joinClassInfor.NewClass.HOC = hoc;
                _context.Classes.Add(joinClassInfor.NewClass);
                RoomDocument roomD = new RoomDocument
                {
                    ClassInfoID = joinClassInfor.NewClass.ClassID
                };
                _context.RoomDocuments.Add(roomD);
                RoomMember roomM = new RoomMember
                {
                    ClassID = joinClassInfor.NewClass.ClassID
                };
                _context.RoomMembers.Add(roomM);
                await _context.SaveChangesAsync();
                return RedirectToAction("Room", "Room", joinClassInfor.NewClass);
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="Student,Teacher")]
        public IActionResult Join(JoinClassInfor joinClassInfor)
        {
            if (ModelState.IsValid)
            {
                Classes cls = _context.Classes.Find(joinClassInfor.NewClass.ClassID);
                if (cls == null)
                    return RedirectToAction("Create","JoinClass", joinClassInfor);
                else {
                    return RedirectToAction("Room", "Room", cls);
                }
            }
            return View(joinClassInfor);
        }
        public IActionResult LoadUserList(string userCalls)
        {
            return PartialView("_BoxChatPartial", userCalls);
        }

        [HttpGet]
        public IActionResult GetInfo(string classid)
        {
            var model = new UserInfo();
            var userId = userManager.GetUserId(this.User);
            var myclass = _context.Classes.Find(classid);
            if (myclass != null) {
                if (myclass.UserId == userId)
                    model.IsCaller = true;
                else model.IsCaller = false;
            }
            model.Name = User.Identity.Name;
            model.ClassId = classid;
            return new JsonResult(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                for (int i = 0; i < bytes.Length; i++)  
                {  
                    builder.Append(bytes[i].ToString("x2"));  
                }  
                return builder.ToString();  
            }  
            
        }
        #region Cookies 
        public void WriteCookies(string setting, string settingValue, bool isPersistent) {
            if(isPersistent)
            {
                CookieOptions options= new CookieOptions();
                options.Expires = DateTime.Now.AddMinutes(60);
                Response.Cookies.Append(setting,settingValue, options);
            }else
            {
                Response.Cookies.Append(setting, settingValue);
            }
            ViewBag.Message = "Cookie Written Successfully!";
        }
        public Classes ReadCookies(){
            var ClassInfo = new Classes();
            ClassInfo.ClassName  = Request.Cookies["ClassName"];
            ClassInfo.ClassID = Request.Cookies["ClassID"];
            ClassInfo.Topic =  Request.Cookies["Topic"];
            return ClassInfo;
        }
        #endregion 
        
    }
}
