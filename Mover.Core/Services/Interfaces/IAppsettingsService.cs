using Mover.Core.Dto.User;
using Mover.Core.Dto.Appsetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mover.Core.Dto.Carts;
using Mover.Core.Dto.Order;
using Mover.Core.Entities;

namespace Mover.Core.Services.Interfaces
{
    public interface IAppsettingsService
    {
        Task<List<AppsettingDto>> GetAppsettingByKey(string key);
        Task<List<AppsettingDto>> GetAppsettingByPage(string page);
        Task SaveOrUpdate(AppsettingDto dto);
        Task SaveBanner(BannerDto dto);
        Task<bool> Delete(int id, string imageUrl);
    }
}
