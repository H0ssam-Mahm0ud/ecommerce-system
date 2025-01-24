using Ecommerce.Models.Models;

namespace Ecommerce.Models.Repository
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        void Update(Category category);
    }
}
