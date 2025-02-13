﻿using Mover.Core.Entities;
using Mover.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Infrastructure.Repository.Implementations
{
    public class UserDetailRepository : BaseRepository<UserDetail>, IUserDetailRepository
    {
        public UserDetailRepository(MoverContext context) : base(context)
        {

        }
    }
}
