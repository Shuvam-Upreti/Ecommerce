using Microsoft.AspNetCore.Identity;
using Mover.Core.Dto.User;
using Mover.Core.Entities.UserManagement;
using Mover.Core.Repository.Interfaces;
using Mover.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserDetailRepository _userDetailRepo;
        public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager, IUserDetailRepository userDetailRepo)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _userDetailRepo = userDetailRepo;
        }
        public async Task<UserSessionDto> GetUserDetailByUsername(string userName)
        {
            var aspUser = await _userManager.FindByNameAsync(userName);
            if (aspUser == null)
                return null;
            var getUserId = _userDetailRepo.GetQueryable().Where(u => u.AspUserId == aspUser.Id).FirstOrDefault();

            var roles = (await _userManager.GetRolesAsync(aspUser)).FirstOrDefault();
            var userDetail = new UserSessionDto()
            {
                AspUserId = aspUser.Id,
                Id = getUserId.Id,
                UserName = aspUser.UserName,
                FullName = getUserId.FullName,
                Role = roles
            };
            return userDetail;

        }
    }
}
