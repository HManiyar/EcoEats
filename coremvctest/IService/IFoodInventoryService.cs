using coremvctest.Models;
using coremvctest.RequestModels;

namespace coremvctest.IService
{
    public interface IFoodInventoryService
    {
        Task<FoodStoreEntity> getFoodInventoryById(FoodInventoryLoginRequestModel foodInventory);
        byte[] HashPassword(string password, byte[] salt, int iterations = 10000);
        bool SlowEquals(byte[] a, byte[] b);
        byte[] GenerateSalt(int length);
    }
}
