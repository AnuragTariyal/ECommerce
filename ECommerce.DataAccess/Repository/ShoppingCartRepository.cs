using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repoistory;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.DataAccess.Repository
{
    public class ShoppingCartRepository : Repoistory<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;
        public ShoppingCartRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }
        public void Update(ShoppingCartRepository shoppingCart)
        {
            _context.Update(shoppingCart);
        }
    }
}
