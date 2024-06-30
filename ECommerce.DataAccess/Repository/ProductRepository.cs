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
    public class ProductRepository : Repoistory<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public void Update(Product product)
        {
            var productInDb = _context.Products.FirstOrDefault(p => p.Id == product.Id);
            if (productInDb.ImageUrl != "")
            {
                productInDb.ImageUrl = product.ImageUrl;
                productInDb.Title = product.Title;
                productInDb.Description = product.Description;
                productInDb.ISBN = product.ISBN;
                productInDb.Author = product.Author;
                productInDb.ListPrice = product.ListPrice;
                productInDb.Price50 = product.Price50;
                productInDb.Price100 = product.Price100;
                productInDb.Price = product.Price;
                productInDb.CategoryId = product.CategoryId;
                productInDb.CoverTypeId = product.CoverTypeId;
            }
            //_context.Update(product);
        }
    }
}
