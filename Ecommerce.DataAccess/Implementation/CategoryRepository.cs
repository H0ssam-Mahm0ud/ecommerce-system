using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Implementation;
using Ecommerce.Models.Models;
using Ecommerce.Models.Repository;

namespace myshop.DataAccess.Implementation
{
    public class CategoryRepository : GenericRepository<Category>,ICategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var categoryFromDb = _context.Categories.FirstOrDefault(x=>x.Id == category.Id);
            if (categoryFromDb != null)
            {
                categoryFromDb.Name = category.Name;
                categoryFromDb.Description = category.Description;
                categoryFromDb.CreatedTime = DateTime.Now;
            }
        }
    }
}
