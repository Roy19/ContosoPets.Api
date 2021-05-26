using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ContosoPets.Api.Models;
using System.Threading.Tasks;

namespace ContosoPets.Api.Data
{
    public class ContosoPetsContext : DbContext, IContosoPetsContext
    {
        public ContosoPetsContext(DbContextOptions<ContosoPetsContext> options) : base(options)
        {
        }

        public DbSet<Product> Products {get; set;}

        public Task<int> SaveChangesAsync()
        {
            return base.SaveChangesAsync();
        }

        public EntityEntry<Product> Entry(Product product)
        {
            return base.Entry(product);
        }
    }
}