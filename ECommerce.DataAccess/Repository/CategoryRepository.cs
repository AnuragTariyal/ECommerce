using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repoistory.IRepository;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repoistory
{

    public class CategoryRepository : Repoistory<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void update(Category category)
        {
            _context.Update(category);
        }
    }
}
