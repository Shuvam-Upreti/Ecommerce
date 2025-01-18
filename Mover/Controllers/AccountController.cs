using Mover.Core.Dto.User;
using Mover.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mover.Core.Entities.UserManagement;
using Mover.Core.Enums.Account;
using Mover.Core.Exceptions;
using Mover.Core.Services.Interfaces;
using Mover.HttpUtility;
using Mover.ViewModel.Account;

namespace Mover.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userService = userService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrEmpty(model.Password))
                {
                    return View(model);
                }

                var changingUser = new ApplicationUser();

                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    var userDetails = await _userService.GetUserDetailByUsername(model.Username);

                    var checkUser = new UserSessionDto
                    {
                        AspUserId = userDetails.AspUserId,
                        UserName = userDetails.UserName,
                        FullName = userDetails.FullName,
                        Id = userDetails.Id,
                        //Role = XmartRecovery.Core.Enums.RolesEnums.Roles.SuperAdmin.ToString()
                    };

                    HttpContext.Session.SetSessionObject(checkUser);
                    changingUser = await _userManager.FindByIdAsync(checkUser.AspUserId);
                    await _signInManager.SignInAsync(changingUser, isPersistent: false);
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        LoginMessage returnType = LoginMessage.None;
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home", new { controller = "Index" });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
                }

            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }

        [AllowAnonymous]
        public async Task<ActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            HttpContext.Session.Remove("UserDetail");

            return RedirectToAction("Login", "Account");
        }
    }
}

