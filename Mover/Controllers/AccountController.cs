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
using Microsoft.AspNetCore.Mvc.Rendering;
using Mover.Core.Constants;
using Mover.Core.Dto.Filter;
using Mover.Core.Enums.Roles;
using Mover.Extension;
using Mover.ViewModel.Filter;
using Mover.ViewModel.User;

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
        [Authorize(Roles = RolesConstant.Admin)]
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = RolesConstant.Admin)]
        public async Task<IActionResult> Create()
        {
            var roles = Enum.GetValues(typeof(RolesEnum))
                  .Cast<RolesEnum>()
                  .Select(r => new SelectListItem
                  {
                      Value = r.ToString(),
                      Text = r.ToString()
                  }).ToList();

            ViewBag.Roles = new SelectList(roles, "Value", "Text");

            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesConstant.Admin)]
        public async Task<IActionResult> Create(UserDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                this.NotifyModelStateErrors();
                return View(model);
            }
            try
            {
                var dto = new UserDetailDto()
                {
                    FullName = model.FullName,
                    UserName = model.FullName,
                    DateOfJoin = model.DateOfJoin,
                    Department = model.Department,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role,
                    Password = model.Password,
                };
                await _userService.Create(dto);
                this.NotifySuccess("User added sucessfully");
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong");
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = RolesConstant.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
                return NotFound();
            try
            {
                var roles = Enum.GetValues(typeof(RolesEnum))
                 .Cast<RolesEnum>()
                 .Select(r => new SelectListItem
                 {
                     Value = r.ToString(),
                     Text = r.ToString()
                 }).ToList();

                ViewBag.Roles = new SelectList(roles, "Value", "Text");

                var dto = await _userService.GetUser(id);
                var model = new UserDetailViewModel
                {
                    FullName = dto.FullName,
                    DateOfJoin = dto.DateOfJoin,
                    Department = dto.Department,
                    Email = dto.Email,
                    PhoneNumber = dto.PhoneNumber,
                    Role = dto.Role,
                };
                return View(model);
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong");
                return RedirectToAction(nameof(Index));
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesConstant.Admin)]
        public async Task<IActionResult> Edit(UserDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var roles = Enum.GetValues(typeof(RolesEnum))
                    .Cast<RolesEnum>()
                    .Select(r => new SelectListItem
                    {
                        Value = r.ToString(),
                        Text = r.ToString()
                    }).ToList();

                ViewBag.Roles = new SelectList(roles, "Value", "Text");
                this.NotifyError("Please correct the errors and try again.");
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Password))
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    var roles = Enum.GetValues(typeof(RolesEnum))
                        .Cast<RolesEnum>()
                        .Select(r => new SelectListItem
                        {
                            Value = r.ToString(),
                            Text = r.ToString()
                        }).ToList();

                    ViewBag.Roles = new SelectList(roles, "Value", "Text");
                    return View(model);
                }
            }

            try
            {
                var dto = new UserDetailDto()
                {
                    Id = model.Id,
                    FullName = model.FullName,
                    DateOfJoin = model.DateOfJoin,
                    Department = model.Department,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Role = model.Role
                };

                if (!string.IsNullOrEmpty(model.Password))
                {
                    dto.Password = model.Password;
                }

                var result = await _userService.UpdateUser(dto);

                if (result)
                {
                    this.NotifySuccess("User details updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    this.NotifyError("Failed to update user details. Please try again.");
                    return View(model);
                }
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong while updating the user.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("An unexpected error occurred.");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [Authorize(Roles = RolesConstant.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                this.NotifyError("Invalid user ID.");
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var result = await _userService.DeleteUser(id);
                if (result)
                {
                    this.NotifySuccess("User deleted successfully.");
                }
                else
                {
                    this.NotifyError("Failed to delete the user.");
                }
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("An unexpected error occurred.");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = RolesConstant.Admin)]
        public async Task<IActionResult> LoadUser(FilterViewModel model)
        {
            try
            {
                var dto = new FilterDto()
                {
                    Search = model.Search,
                    PageSize = model.PageSize,
                    PageIndex = model.PageIndex
                };
                int totalCount = 0;

                var userList = await _userService.GetAllUser(dto);
                var datas = userList.Select(x => new UserDetailViewModel()
                {
                    Id = x.Id,
                    AspUserId = x.AspUserId,
                    DateOfJoin = x.DateOfJoin,
                    Department = x.Department,
                    Email = x.Email,
                    FullName = x.FullName,
                    Role = x.Role,
                    UserName = x.UserName,
                    PhoneNumber = x.PhoneNumber

                })
                .ToList();

                var result = Json(new { data = datas, totalCount = totalCount });
                return result;

            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong");
                return RedirectToAction(nameof(Index));
            }
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
                        Role = userDetails.Role,
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

