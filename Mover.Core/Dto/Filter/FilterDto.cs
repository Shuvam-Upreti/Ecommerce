using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Core.Dto.Filter
{
    public class FilterDto
    {
        public string? Search { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; } = 20;
    }
}
