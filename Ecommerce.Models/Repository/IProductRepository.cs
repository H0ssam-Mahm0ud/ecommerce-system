using Ecommerce.Models.Models;

namespace Ecommerce.Models.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        void Update(Product product);
    }
}
