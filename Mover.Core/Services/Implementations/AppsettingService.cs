using Mover.Core.Dto.User;
using Mover.Core.Entities;
using Mover.Core.Entities.UserManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mover.Core.Dto.Category;
using Mover.Core.Helpers;
using Mover.Core.Repository.Interfaces;
using Mover.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mover.Core.Exceptions;
using Mover.Core.Dto.Appsetting;
using Mover.Core.Dto.Order;
using Mover.Core.Enums.Appsetting;

namespace Mover.Core.Services.Implementations
{
    public class AppsettingService : IAppsettingsService
    {
        private readonly IAppsettingsRepository _appsettingsRepository;
        private readonly IFileHelper _fileHelper;
        public AppsettingService(IAppsettingsRepository appsettingsRepository, IFileHelper fileHelper)
        {
            _appsettingsRepository=appsettingsRepository;
            _fileHelper=fileHelper;
        }

        public async Task<List<AppsettingDto>> GetAppsettingByKey(string key)
        {
            var appsetting = await _appsettingsRepository.GetQueryable().Where(x => x.Key == key).ToListAsync();
            if (appsetting == null)
            {
                throw new CustomException("Appsetting not found");
            }

            var dto = appsetting.Select(a => new AppsettingDto()
            {
                Id=a.Id,
                Key = a.Key,
                Value = a.Value
            }).ToList();

            return dto;
        }
        public async Task SaveOrUpdate(AppsettingDto dto)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var appsetting = await _appsettingsRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (appsetting == null)
            {
                var model = new Appsetting()
                {
                    Key = dto.Key,
                    Value = dto.Value
                };
                await _appsettingsRepository.InsertAsync(model);
            }
            else
            {

                appsetting.Key = dto.Key;
                appsetting.Value = dto.Value;
                _appsettingsRepository.Update(appsetting);
            }
            tx.Complete();
        }
        public async Task SaveBanner(BannerDto dto)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var appsetting = await _appsettingsRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (appsetting == null)
            {
                var model = new Appsetting()
                {
                    Key = AppsettingEnum.BannerImage.ToString(),
                    Value = dto.ImageUrl
                };
                await _appsettingsRepository.InsertAsync(model);
            }
            else
            {

                appsetting.Value = dto.ImageUrl;
                _appsettingsRepository.Update(appsetting);
            }
            tx.Complete();
        }
        public async Task<bool> Delete(int id, string imageUrl)
        {
            using var tx = TransactionScopeHelper.GetInstance();
            var appsetting = await _appsettingsRepository.GetQueryable().FirstOrDefaultAsync(x => x.Id == id);
            if (appsetting == null)
            {
                throw new CustomException("Appsetting not found");
            }
            await _fileHelper.DeleteImageAsync(imageUrl, "uploads/banner");
            _appsettingsRepository.Delete(appsetting);

            tx.Complete(); return true;
        }

    }
}
