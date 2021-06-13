using AppEducation.Models;
using AppEducation.Models.RoomInfo;
using AppEducation.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AppEducation.Controllers
{
    public class RoomController : Controller
    {
        private AppIdentityDbContext _context;
        private UserManager<AppUser> _userManager;
        public RoomController(AppIdentityDbContext context,UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Room(Classes cls)
        {
            return View(cls);
        }
        public IActionResult Member(Classes cls)
        {
            return View(cls);
        }
        public IActionResult Exercise(Classes cls)
        {

            return View(cls);
        }
        public IActionResult Document(Classes cls)
        {
            var roomD = _context.RoomDocuments.Where(t => t.ClassInfoID == cls.ClassID).FirstOrDefault();
            var docs = _context.Documents.Where(t => t.RoomDocumentID == roomD.RoomDocumentID).ToList();
            DocumentViewModel docview = new DocumentViewModel
            {
                myClass = cls,
                myDocuments = docs
            };

            return View(cls);
        }
        [HttpPost]
        public async Task<IActionResult> GetDocuments(string classid)
        {
            var cls = _context.Classes.Find(classid);
            var roomD = _context.RoomDocuments.Where(t => t.ClassInfoID == cls.ClassID).FirstOrDefault();
            var docs = _context.Documents.Where(t => t.RoomDocumentID == roomD.RoomDocumentID).ToList();
            DocumentViewModel model = new DocumentViewModel
            {
                myClass = cls,
                myDocuments = docs
            };
            return new JsonResult(model);
        }
        public IActionResult Present(Classes cls)
        {
            return View(cls);
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot/Data",
                        file.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            AppUser currentUser = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            var cls = _context.Classes.Where(t => t.UserId == currentUser.Id).FirstOrDefault();
            var roomD = _context.RoomDocuments.Where(t => t.ClassInfoID == cls.ClassID).FirstOrDefault();
            Document nD = new Document
            {
                User = currentUser,
                Path = file.FileName,
                RoomDocuments = roomD
            };
            _context.Documents.Add(nD);
            await _context.SaveChangesAsync();
            return new JsonResult("Success");
        }
        public async Task<IActionResult> Download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(),
                           "wwwroot/Data/", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(path), Path.GetFileName(path));
        }
        private string GetContentType(string path)
        {
            var types = GetMimeTypes();
            var ext = Path.GetExtension(path).ToLowerInvariant();
            return types[ext];
        }

        private Dictionary<string, string> GetMimeTypes()
        {
            return new Dictionary<string, string>
            {
                {".txt", "text/plain"},
                {".pdf", "application/pdf"},
                {".doc", "application/vnd.ms-word"},
                {".docx", "application/vnd.ms-word"},
                {".xls", "application/vnd.ms-excel"},
                {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},  
                {".png", "image/png"},
                {".jpg", "image/jpeg"},
                {".jpeg", "image/jpeg"},
                {".gif", "image/gif"},
                {".csv", "text/csv"}
            };
        }
    }
}
