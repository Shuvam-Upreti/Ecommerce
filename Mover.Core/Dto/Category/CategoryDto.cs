using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Dto.Category
{
    public class CategoryDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public List<IFormFile> Images { get; set; } = new List<IFormFile>();

        public DateTime CreatedOn { get; set; }
    }
}
