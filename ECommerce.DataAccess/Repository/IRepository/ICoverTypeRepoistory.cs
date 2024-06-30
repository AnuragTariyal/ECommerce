using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repoistory.IRepository
{
    public interface ICoverTypeRepoistory:IRepository<CoverType>
    {
        void Update(CoverType coverType);
    }
}
