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
    public class CoverTypeRepository : Repoistory<CoverType>, ICoverTypeRepoistory
    {
        private readonly ApplicationDbContext _context;
        public CoverTypeRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public void Update(CoverType coverType)
        {
            _context.Update(coverType);
        }
    }
}
