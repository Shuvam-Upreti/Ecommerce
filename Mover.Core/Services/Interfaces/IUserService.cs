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
    }
}
