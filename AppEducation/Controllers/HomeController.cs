using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AppEducation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using AppEducation.Models.Users;
using System.ComponentModel.DataAnnotations;


namespace AppEducation.Controllers
{
    
   
    public class HomeController : Controller
    {
        private readonly AppIdentityDbContext _context;
        private readonly ILogger<HomeController> _logger;
      
        public HomeController(ILogger<HomeController> logger,AppIdentityDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index()
        {
            
            return View();
        }
        #region Cookies 
        
        #endregion 

    }
}
