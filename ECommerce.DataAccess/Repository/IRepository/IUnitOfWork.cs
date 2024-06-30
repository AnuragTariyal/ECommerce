using ECommerce.DataAccess.Repository.IRepository;

namespace ECommerce.DataAccess.Repoistory.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository category { get; }
        ICoverTypeRepoistory coverType { get; }
        ISP_CALL SP_CALL { get; }
        ICompanyRepository Company {get;}
        IProductRepository Product { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderDetailRepoistory OrderDetail { get; }
        IOrderHeaderRepoistory OrderHeader { get; }
        
        void Save();
    }
}