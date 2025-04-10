using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Dto.Appsetting
{
    public class BannerDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public IFormFile Image { get; set; }
    }
}

