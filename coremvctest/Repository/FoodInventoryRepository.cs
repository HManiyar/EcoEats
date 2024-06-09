using coremvctest.Data;
using coremvctest.IRepository;
using coremvctest.Models;
using coremvctest.RequestModels;
using Microsoft.EntityFrameworkCore;

namespace coremvctest.Repository
{
    public class FoodInventoryRepository : IFoodInventoryRepository
    {
        private readonly ApplicationDbContext _db;
        public FoodInventoryRepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<FoodStoreEntity> getFoodInventoryById(FoodInventoryLoginRequestModel foodStore)
        {
           return await _db.FoodInventories.SingleOrDefaultAsync(m =>
        m.FoodInventoryUserName == foodStore.FoodInventoryUserName)??new FoodStoreEntity();
        }
    }
}
