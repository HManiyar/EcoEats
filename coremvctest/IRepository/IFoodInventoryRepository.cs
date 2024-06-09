using coremvctest.Models;
using coremvctest.RequestModels;

namespace coremvctest.IRepository
{
    public interface IFoodInventoryRepository
    {
        Task<FoodStoreEntity> getFoodInventoryById(FoodInventoryLoginRequestModel foodStore);
    }
}
