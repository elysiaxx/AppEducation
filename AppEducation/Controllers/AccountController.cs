using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppEducation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using AppEducation.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication;
namespace AppEducation.Controllers
{
    public class AccountController : Controller
    {
        private AppIdentityDbContext context;
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private IUserValidator<AppUser> userValidator;
        private IPasswordValidator<AppUser> passwordValidator;
        private IPasswordHasher<AppUser> passwordHasher;
        private readonly ILogger<AccountController> logger;

        public AccountController(AppIdentityDbContext context, SignInManager<AppUser> signInManager, IPasswordHasher<AppUser> passwordHasher, IUserValidator<AppUser> userValidator, IPasswordValidator<AppUser> passwordValidator, UserManager<AppUser> userManager, ILogger<AccountController> logger)
        {
            this.context = context;
            this.passwordValidator = passwordValidator;
            this.passwordHasher = passwordHasher;
            this.userValidator = userValidator;
            this.userManager = userManager;
            this.logger = logger;
            this.signInManager = signInManager;
        }
        [Authorize(Roles = "Student, Teacher")]
        public IActionResult Index() => RedirectToAction("Profile");

        #region Register method
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
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
                    FullName = model.UserName,
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
                    return RedirectToAction("Index", "Home");
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
        #endregion

        #region Login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel, string returnUrl)
        {
            if (string.IsNullOrEmpty(loginModel.UserName))
            {
                ModelState.AddModelError("", "Please enter your email or username");
            }
            if (string.IsNullOrEmpty(loginModel.Password))
            {
                ModelState.AddModelError("", "Please enter your password");
            }
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(loginModel.UserName);
                if (user != null)
                {
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, loginModel.Password, false, false);
                    if (result.Succeeded)
                    {
                        if (user.UserName == "admin")
                        {
                            return RedirectToAction(loginModel.RequestPath ?? "Index", "Admin");
                        }
                        else
                            return Redirect(loginModel.RequestPath ?? "/JoinClass/Create");
                    }
                }
                ModelState.AddModelError(nameof(LoginModel.Email), "Invalid user or password");
            }
            return View(loginModel);

        }
        #endregion 

        #region Logout
        [Authorize]
        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion 
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> Profile()
        {

            AppUser currentUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
            UserProfile profile = context.UserProfiles.First(p => p.UserId == currentUser.Id);
            return View(profile);
        }
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student,Teacher")]
        public async Task<IActionResult> ChangeProfile(UserProfile profile)
        {
            if (ModelState.IsValid)
            {
                AppUser currentUser = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);
                
                UserProfile old = context.UserProfiles.First(p => p.UserId == currentUser.Id);
                if (old != null)
                {
                    if( old.FullName != profile.FullName)
                    {
                        old.FullName = profile.FullName;
                    }
                    if( old.Birthday != profile.Birthday)
                    {
                        old.Birthday = profile.Birthday;
                    }
                    if( old.PhoneNumber != profile.PhoneNumber)
                    {
                        currentUser.PhoneNumber = profile.PhoneNumber;
                    }
                    if( old.Job != profile.Job)
                    {
                        old.Job = profile.Job;
                    }
                    if(old.Sex != profile.Sex)
                        old.Sex = profile.Sex;
                    if (old.Password != profile.Password)
                    {
                        var token = await userManager.GeneratePasswordResetTokenAsync(currentUser);

                        var res = await userManager.ResetPasswordAsync(currentUser, token, profile.Password);
                        if (res.Succeeded)
                        {
                            old.Password = profile.Password;
                        }
                    }
                    context.SaveChanges();
                    var result = await userManager.UpdateAsync(currentUser);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        ModelState.AddModelError("Error", "Change not success!");
                    }
                }

            }
            return RedirectToAction("Profile");
        }

    }
}