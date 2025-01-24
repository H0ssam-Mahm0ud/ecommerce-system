using Ecommerce.DataAccess.Data;
using Ecommerce.DataAccess.Implementation;
using Ecommerce.Models.Models;
using Ecommerce.Models.Repository;

namespace myshop.DataAccess.Implementation
{
    public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
