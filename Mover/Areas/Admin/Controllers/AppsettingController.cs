using Microsoft.AspNetCore.Mvc;
using Mover.Core.Dto.Appsetting;
using Mover.Core.Enums.Appsetting;
using Mover.Core.Exceptions;
using Mover.Core.Helpers;
using Mover.Core.Services.Implementations;
using Mover.Core.Services.Interfaces;
using Mover.Extension;
using Mover.Logging;
using Mover.ViewModel.Appsetting;
using Mover.ViewModel.Banner;

namespace Mover.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AppsettingController : Controller
    {
        private readonly IAppsettingsService _appsettingService;
        private readonly IFileHelper _fileHelper;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public AppsettingController(IAppsettingsService appsettingService, IFileHelper fileHelper, IWebHostEnvironment hostingEnvironment)
        {
            _appsettingService=appsettingService;
            _fileHelper=fileHelper;
            _hostingEnvironment=hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        } 
        public IActionResult AboutSettings()
        {
            return View();
        }

        public async Task<IActionResult> Banner()
        {
            try
            {
                var banner = await _appsettingService.GetAppsettingByKey(AppsettingEnum.BannerImage.ToString());
                var vm = banner.Select(a => new BannerViewModel()
                {
                    Id = a.Id,
                    ImageUrl = a.Value,

                }).ToList();
                return View(vm);

            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdateBanner(BannerDto dto)
        {
            try
            {
                if (dto.Image == null || dto.Image.Length == 0)
                {
                    this.NotifyError("Please select an image to upload.");
                    return RedirectToAction("Banner");
                }

                if (!_fileHelper.IsImageValid(dto.Image.FileName))
                {
                    this.NotifyError("Invalid image file type. Allowed types: .jpeg, .png, .jpg, .gif, .webp");
                    return RedirectToAction("Banner");
                }

                // Save image in wwwroot/uploads/banner
                var destinationFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/banner");
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                var savedFileName = await _fileHelper.SaveImageAndGetFileName(dto.Image, destinationFolder);
                var imageUrl = $"/uploads/banner/{savedFileName}";

                // Prepare AppsettingDto for saving
                var settingDto = new AppsettingDto
                {
                    Id = dto.Id,
                    Key = AppsettingEnum.BannerImage.ToString(),
                    Page = AppsettingEnum.BannerImage.ToString(),
                    Value = imageUrl
                };

                await _appsettingService.SaveOrUpdate(settingDto);
                this.NotifySuccess("Banner image saved successfully.");
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong. Please try again.");
            }

            return RedirectToAction("Banner");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteBanner(int id, string imageUrl)
        {
            try
            {
               
                var isDeleted = await _appsettingService.Delete(id, imageUrl);

                if (isDeleted)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error deleting the banner." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                new SeriLogger().Error(ex.Message, ex);
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }

        public async Task<IActionResult> BrandSettings()
        {
            try
            {
                var banner = await _appsettingService.GetAppsettingByKey(AppsettingEnum.BrandImage.ToString());
                var vm = banner.Select(a => new BannerViewModel()
                {
                    Id = a.Id,
                    ImageUrl = a.Value,

                }).ToList();
                return View(vm);

            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
                return View();
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong.Please try again");
                return View();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdateBrandImage(BannerDto dto)
        {
            try
            {
                if (dto.Image == null || dto.Image.Length == 0)
                {
                    this.NotifyError("Please select an image to upload.");
                    return RedirectToAction("Banner");
                }

                if (!_fileHelper.IsImageValid(dto.Image.FileName))
                {
                    this.NotifyError("Invalid image file type. Allowed types: .jpeg, .png, .jpg, .gif, .webp");
                    return RedirectToAction("BrandSetinga");
                }

                // Save image in wwwroot/uploads/banner
                var destinationFolder = Path.Combine(_hostingEnvironment.WebRootPath, "uploads/brands");
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                var savedFileName = await _fileHelper.SaveImageAndGetFileName(dto.Image, destinationFolder);
                var imageUrl = $"/uploads/brands/{savedFileName}";

                // Prepare AppsettingDto for saving
                var settingDto = new AppsettingDto
                {
                    Id = dto.Id,
                    Key = AppsettingEnum.BrandImage.ToString(),
                    Page = AppsettingEnum.BrandImage.ToString(),
                    Value = imageUrl
                };

                await _appsettingService.SaveOrUpdate(settingDto);
                this.NotifySuccess("Banner image saved successfully.");
            }
            catch (CustomException ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError(ex.Message);
            }
            catch (Exception ex)
            {
                new SeriLogger().Error(ex.Message, ex);
                this.NotifyError("Something went wrong. Please try again.");
            }

            return RedirectToAction("BrandSettings");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteBrandImage(int id, string imageUrl)
        {
            try
            {

                var isDeleted = await _appsettingService.Delete(id, imageUrl);

                if (isDeleted)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Error deleting the brand image." });
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                new SeriLogger().Error(ex.Message, ex);
                return Json(new { success = false, message = "Something went wrong. Please try again." });
            }
        }
    }
}
