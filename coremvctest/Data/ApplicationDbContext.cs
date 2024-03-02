using coremvctest.Models;
using Microsoft.EntityFrameworkCore;

namespace coremvctest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<FoodStoreEntity> FoodInventories { get; set; }
        public DbSet<NGOEntity> NGOs { get; set; }
        public DbSet<FoodsEntity> Foods { get; set; }
        public DbSet<ExpireWarnings> ExpireWarnings { get; set; }
        public DbSet<RequestedFoodsByNGO> RequestedFoodsByNGO { get; set; }
        public DbSet<RequestedInquiryFoodsResult> RequestedInquiryFoodsResult { get; set; }
        public DbSet<LocationEntity> Locations { get; set; }

    }
}
