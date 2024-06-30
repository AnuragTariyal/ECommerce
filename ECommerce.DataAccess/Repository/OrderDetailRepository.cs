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
    public class OrderDetailRepository : Repoistory<OrderDetail>, IOrderDetailRepoistory
    {
        private readonly ApplicationDbContext _context;
        public OrderDetailRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(OrderDetail orderDetail)
        {
            _context.OrderDetails.Update(orderDetail);
        }
    }
}
