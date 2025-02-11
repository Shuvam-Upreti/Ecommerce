using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Dto.User
{
    public class UserDetailDto
    {
        public int Id { get; set; }

        public string AspUserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfJoin { get; set; }
        public string? Department { get; set; }
        public string Role { get; set; }
        //public SelectList Roles { get; set; }
        public string Email { get; set; }
    }
}
