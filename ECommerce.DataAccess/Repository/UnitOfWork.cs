using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repoistory.IRepository;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;

namespace ECommerce.DataAccess.Repoistory
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            category = new CategoryRepository(_context);
            coverType=new CoverTypeRepository(_context);
            Product = new ProductRepository(_context);
            SP_CALL = new SP_CALL(_context);
            Company = new CompanyRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            ShoppingCart = new ShoppingCartRepository(_context);
            OrderDetail = new OrderDetailRepository(_context);
            OrderHeader = new OrderHeaderRepository(_context);
        } 
        public ICategoryRepository category { get; private set; }
        public ICoverTypeRepoistory coverType { get; private set; }
        public ISP_CALL SP_CALL { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }

        public IOrderDetailRepoistory OrderDetail { get; private set; }

        public IOrderHeaderRepoistory OrderHeader { get; private set; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
