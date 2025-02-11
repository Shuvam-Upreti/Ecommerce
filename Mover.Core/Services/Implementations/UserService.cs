using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mover.Core.Dto.Filter;
using Mover.Core.Dto.User;
using Mover.Core.Entities;
using Mover.Core.Entities.UserManagement;
using Mover.Core.Exceptions;
using Mover.Core.Helpers;
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
        public async Task Create(UserDetailDto model)
        {
            using var tx = TransactionScopeHelper.GetInstance();

            var applicationUser = new ApplicationUser
            {
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email,
            };

            var createAspUserResult = await _userManager.CreateAsync(applicationUser, model.Password);
            if (!createAspUserResult.Succeeded)
            {
                var errorMessages = createAspUserResult.Errors.Select(error => error.Description);
                throw new CustomException(string.Join(", ", errorMessages));

            }

            var userDetail = new UserDetail
            {
                FullName = model.FullName,
                DateOfJoin = model.DateOfJoin,
                Department = model.Department,
                AspUserId = applicationUser.Id
            };

            await _userDetailRepo.InsertAsync(userDetail);
            await _userManager.AddToRoleAsync(applicationUser, model.Role);

            tx.Complete();
        }

        public async Task<List<UserDetailDto>> GetAllUser(FilterDto filter)
        {
            var userList = await _userDetailRepo.GetAllAsync();
            var totalCount = userList.Count();
            var pagedData = userList.Skip(filter.PageIndex).Take(filter.PageSize);
            var dtoList = pagedData.Select(x => new UserDetailDto
            {
                Id = x.Id,
                AspUserId = x.AspUserId,
                DateOfJoin = x.DateOfJoin,
                Department = x.Department,
                Email = x.AspUser.Email,
                FullName = x.FullName,
                PhoneNumber = x.AspUser.PhoneNumber,
                Role = string.Join(", ", x.AspUser.Roles.Select(role => role.Name)),
                UserName = x.AspUser.UserName
            }).ToList();
            return dtoList;
        }

        public async Task<UserDetailDto> GetUser(int id)
        {
            var user = await _userDetailRepo.GetByIdAsync(id) ?? throw new CustomException("No user found.");
            var model = new UserDetailDto
            {
                FullName = user.FullName,
                DateOfJoin = user.DateOfJoin,
                Department = user.Department,
                Email = user.AspUser.Email,
                PhoneNumber = user.AspUser.PhoneNumber,
                Role = string.Join(", ", user.AspUser.Roles.Select(r => r.Name)),
            };
            return model;
        }
        public async Task<bool> UpdateUser(UserDetailDto dto)
        {
            using var tx = TransactionScopeHelper.GetInstance();

            var userDetail = await _userDetailRepo.GetQueryable().Where(a => a.Id == dto.Id).FirstOrDefaultAsync() ?? throw new CustomException("User Detail not found.");
            var user = await _userManager.FindByIdAsync(userDetail.AspUserId);
            if (user == null)
            {
                throw new CustomException("User not found.");
            }

            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.UserName = dto.Email;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new CustomException("Failed to update user details.");
            }

            if (!string.IsNullOrEmpty(dto.Password))
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    throw new CustomException("Failed to remove old password.");
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, dto.Password);
                if (!addPasswordResult.Succeeded)
                {
                    throw new CustomException("Failed to set new password.");
                }
            }
            if (!string.IsNullOrEmpty(dto.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    throw new CustomException("Failed to remove existing roles.");
                }

                var addRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!addRoleResult.Succeeded)
                {
                    throw new CustomException($"Failed to assign role '{dto.Role}'.");
                }
            }
            userDetail.FullName = dto.FullName;
            userDetail.DateOfJoin = dto.DateOfJoin;
            userDetail.Department = dto.Department;
            _userDetailRepo.Update(userDetail);
            tx.Complete();
            return true;
        }
        public async Task<bool> DeleteUser(int userId)
        {
            using var tx = TransactionScopeHelper.GetInstance();

            var userDetail = await _userDetailRepo.GetQueryable().Where(a => a.Id == userId).FirstOrDefaultAsync() ?? throw new CustomException("User Detail not found.");
            var user = await _userManager.FindByIdAsync(userDetail.AspUserId);
            if (user == null)
            {
                throw new CustomException("User not found.");
            }
            await _userDetailRepo.DeleteAsync(userDetail);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new CustomException($"Failed to delete user. Errors: {errorMessages}");
            }

            tx.Complete();
            return true;
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
