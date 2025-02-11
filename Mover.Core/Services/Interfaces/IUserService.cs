using Mover.Core.Dto.Filter;
using Mover.Core.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserSessionDto> GetUserDetailByUsername(string userName); 
        Task<UserDetailDto> GetUser(int id);
        Task<List<UserDetailDto>> GetAllUser(FilterDto model);
        Task Create(UserDetailDto model);
        Task<bool> UpdateUser(UserDetailDto dto);
        Task<bool> DeleteUser(int userId);
    }
}
