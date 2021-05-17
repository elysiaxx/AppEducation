using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppEducation.Models.Users;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AppEducation.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace AppEducation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;
        private TotalInformation totalInfo;
        private AppIdentityDbContext context;
        private List<Room> rooms;
        public AdminController(UserManager<AppUser> usrMgr, List<Room> rms, AppIdentityDbContext ctx)
        {
            context = ctx;
            userManager = usrMgr;
            rooms = rms;
        }
        public ViewResult Index()
        {
            totalInfo = new TotalInformation();
            totalInfo.Users = userManager.Users;
            totalInfo.Rooms = rooms;
            return View(totalInfo);
        }
        public ViewResult UserManager() => View(userManager.Users);
        public IActionResult ClassManager()
        {
            IEnumerable<Classes> cls = context.Classes;
            return View(cls);
        }
        [HttpGet]
        public async Task<IActionResult> DetailsTime(string id)
        {
            Classes classes = await context.Classes.FindAsync(id);

            HistoryOfClass profile = context.HOClasses.SingleOrDefault(p => p.hocID == classes.ClassID);

            return View(profile);
        }
        /*        public async Task<JsonResult> detailasjson(string id)
                {
                    AppUser user = await userManager.FindByIdAsync(id);

                    if (user != null)
                    {
                        UserProfile profile = context.UserProfiles.First(p => p.UserId == user.Id);
                        string data = JsonConvert.SerializeObject(profile);
                        return Json(data);
                    }
                    else
                    {
                        return Json(null);
                    }
                }*/
        public async Task<IActionResult> EditProfile(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            UserProfile profile = context.UserProfiles.SingleOrDefault(p => p.UserId == user.Id);

            return RedirectToAction("SaveProfile","Admin", profile);
        }
        public IActionResult SaveProfile(UserProfile profile)
        {
            return View(profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProfilePost(UserProfile profile)
        {
            if (ModelState.IsValid)
            {

                AppUser appUser = await context.Users.FindAsync(profile.UserId);
                UserProfile old = context.UserProfiles.SingleOrDefault(p => p.Email == profile.Email);
                if (old != null)
                {
                    if (old.FullName != profile.FullName)
                    {
                        old.FullName = profile.FullName;
                    }
                    if (old.Birthday != profile.Birthday)
                    {
                        old.Birthday = profile.Birthday;
                    }
                    if (old.PhoneNumber != profile.PhoneNumber)
                    {
                        old.PhoneNumber = profile.PhoneNumber;
                    }
                    if (old.Job != profile.Job)
                    {
                        old.Job = profile.Job;
                    }
                    if (old.Sex != profile.Sex)
                        old.Sex = profile.Sex;
                    if (old.Password != profile.Password)
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(appUser);

                        var res = await userManager.ResetPasswordAsync(appUser, token, profile.Password);
                        if (res.Succeeded)
                        {
                            old.Password = profile.Password;
                        }
                    }
                    context.SaveChanges();
                    var result = await userManager.UpdateAsync(appUser);
                    if (result.Succeeded)
                    {                       
                        return RedirectToAction("UserManager");
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "Change not success!");
                    }
                }

            }
            return RedirectToAction("UserManager");
        }
        //public async Task<IActionResult> EditProfile(string id)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AppUser currentUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

        //        UserProfile old = context.UserProfiles.First(p => p.UserId == currentUser.Id);
        //        if (old != null)
        //        {
        //            if (old.FullName != profile.FullName)
        //            {
        //                old.FullName = profile.FullName;
        //            }
        //            if (old.Birthday != profile.Birthday)
        //            {
        //                old.Birthday = profile.Birthday;
        //            }
        //            if (old.PhoneNumber != profile.PhoneNumber)
        //            {
        //                currentUser.PhoneNumber = profile.PhoneNumber;
        //            }
        //            if (old.Job != profile.Job)
        //            {
        //                old.Job = profile.Job;
        //            }
        //            if (old.Sex != profile.Sex)
        //                old.Sex = profile.Sex;
        //            if (old.Password != profile.Password)
        //            {
        //                var token = await userManager.GeneratePasswordResetTokenAsync(currentUser);

        //                var res = await userManager.ResetPasswordAsync(currentUser, token, profile.Password);
        //                if (res.Succeeded)
        //                {
        //                    old.Password = profile.Password;
        //                }
        //            }
        //            context.SaveChanges();
        //            var result = await userManager.UpdateAsync(currentUser);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Profile");
        //            }
        //            else
        //            {
        //                ModelState.AddModelError("Error", "Change not success!");
        //            }
        //        }

        //    }
        //    return RedirectToAction("Profile");
        //}

        public async Task<ViewResult> ShowClassOnline()
        {
           
            List<Classes> classes = context.Classes.ToList();
            List<ClassInfo> classInfos = new List<ClassInfo>();
            if (classes.Count() != 0)
            {
                foreach(Classes class_ in classes)
                {
                    ClassInfo _classinfor = new ClassInfo();
                    if(class_.isActive == true)
                    {
                        _classinfor.ClassID = class_.ClassID;
                        _classinfor.ClassName = class_.ClassName;
                        _classinfor.Topic = class_.Topic;
                        _classinfor.TeacherId = class_.UserId;
                        _classinfor.OnlineStudent = class_.OnlineStudent;
                        classInfos.Add(_classinfor);
                    }
                }
            }
            if(classInfos.Count() != 0)
            {
                foreach(ClassInfo _classinfo in classInfos)
                {
                    AppUser user = await userManager.FindByIdAsync(_classinfo.TeacherId);
                    _classinfo.TeacherId = user.UserName;
                }
            }
            return View(classInfos);
        }
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            UserProfile profile = context.UserProfiles.SingleOrDefault(p => p.UserId == user.Id);

            return View(profile);
        }
        // Delete User 
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            IQueryable<UserProfile> profiles = context.UserProfiles.Where(p => p.UserId == user.Id);
            foreach (UserProfile profile in profiles)
            {
                context.UserProfiles.Remove(profile);
            }
            await context.SaveChangesAsync();
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {

                    return RedirectToAction("Index");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", userManager.Users);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(RegisterModel request)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        AppUser user = new AppUser
        //        {
        //            UserName = request.UserName,
        //            Email = request.Email,
        //            PhoneNumber = request.PhoneNumber
        //        };
        //        UserProfile profile = new UserProfile
        //        {
        //            Birthday = request.Birthday,
        //            Job = request.Job,
        //            Password = request.Password,
        //            PhoneNumber = request.PhoneNumber,
        //            Sex = request.Sex,
        //            Email = request.Email
        //        };

        //        IdentityResult result = await userManager.CreateAsync(user, profile.Password);
        //        if (result.Succeeded)
        //            return RedirectToAction("Index");
        //        else
        //        {
        //            foreach (IdentityError error in result.Errors)
        //                ModelState.AddModelError("", error.Description);
        //        }
        //    }
        //    return View(request);
        //}
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.Birthday))
            {
                ModelState.AddModelError(nameof(model.Birthday), "Please enter your birthday");
            }
            if (ModelState.GetValidationState("Date") == ModelValidationState.Valid && DateTime.Now > Convert.ToDateTime(model.Birthday))
            {
                ModelState.AddModelError(nameof(model.Birthday), "Please enter a date in the past");
            }
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Please enter your email");

            }
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };
                UserProfile profile = new UserProfile
                {
                    Birthday = model.Birthday,
                    Job = model.Job,
                    Password = model.Password,
                    PhoneNumber = model.PhoneNumber,
                    Sex = model.Sex,
                    Email = model.Email
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    // save info user
                    AppUser usernew = await userManager.FindByNameAsync(user.UserName);
                    profile.UserId = usernew.Id;
                    context.UserProfiles.Add(profile);
                    if (profile.Job == "Teacher")
                    {
                        var roleResult = await userManager.AddToRoleAsync(usernew, "Teacher");
                    }
                    else
                    {
                        var roleResult = await userManager.AddToRoleAsync(usernew, "Student");
                    }
                    context.SaveChanges();
                    return RedirectToAction("UserManager", "Admin");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }
    }
}