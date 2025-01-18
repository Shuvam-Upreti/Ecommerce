using Mover.Core.Entities;
using Mover.Core.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mover.Infrastructure.Repository.Implementations
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(MoverContext context) : base(context)
        {

        }
    }
}
