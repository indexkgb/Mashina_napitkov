using System.Data.Entity;
using Machine.Models;
 
namespace Machine.Concrete
{
    public class EFDbContext : DbContext
    {
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Coin> Coins { get; set; }
    }
}