using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Implementation;
using Ecommerce.Models.Models;
using Ecommerce.Models.Repository;
using System.Linq.Expressions;

namespace myshop.DataAccess.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var productFromDb = _context.Products.FirstOrDefault(x=>x.Id == product.Id);
            if (productFromDb != null)
            {
                productFromDb.Name = product.Name;
                productFromDb.Description = product.Description;
                productFromDb.Price = product.Price;
                productFromDb.Image = product.Image;
                productFromDb.CategoryId = product.CategoryId;
            }
        }
    }
}
