using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repoistory;
using ECommerce.DataAccess.Repoistory.IRepository;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class OrderHeaderRepository : Repoistory<OrderHeader>, IOrderHeaderRepoistory
    {
        private readonly ApplicationDbContext _context;
        public OrderHeaderRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(OrderHeader orderHeader)
        {
            _context.OrderHeaders.Update(orderHeader);
        }
    }
}
