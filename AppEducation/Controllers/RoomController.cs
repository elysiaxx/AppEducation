using AppEducation.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppEducation.Controllers
{
    public class RoomController : Controller
    {
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
            return View(cls);
        }
        public IActionResult Present(Classes cls)
        {
            return View(cls);
        }
    }
}
