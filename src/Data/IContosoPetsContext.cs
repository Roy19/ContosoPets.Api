using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ContosoPets.Api.Models;
using System.Threading.Tasks;

namespace ContosoPets.Api.Data
{
    public interface IContosoPetsContext
    {
        DbSet<Product> Products {get; set;}
        Task<int> SaveChangesAsync();
        EntityEntry<Product> Entry(Product product);
    }
}