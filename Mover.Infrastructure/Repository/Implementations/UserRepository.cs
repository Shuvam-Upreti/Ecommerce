using Mover.Core.Entities;
using Mover.Core.Repository.Interfaces;
using Mover.Infrastructure.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Infrastructure.Repository.Implementations
{
    public class UserRepository : BaseRepository<AspNetUser>, IUserRepository
    {
        public UserRepository(MoverContext context) : base(context)
        {
        }
    }
}
