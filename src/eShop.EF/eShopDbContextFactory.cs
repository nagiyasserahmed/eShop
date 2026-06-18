using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace eShop.EF
{
    public class eShopDbContextFactory : IDesignTimeDbContextFactory<eShopDbContext>
    {
        public eShopDbContext CreateDbContext(string[] args = null!)
        {
            var builderOptions = new DbContextOptionsBuilder<eShopDbContext>();

            builderOptions.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=eShopDb;Trusted_Connection=True;");

            return new eShopDbContext(builderOptions.Options);
        }
    }
}
