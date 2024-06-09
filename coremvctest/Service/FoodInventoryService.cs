using coremvctest.IRepository;
using coremvctest.IService;
using coremvctest.Models;
using coremvctest.RequestModels;
using System.Security.Cryptography;

namespace coremvctest.Service
{
    public class FoodInventoryService : IFoodInventoryService
    {
        private readonly IFoodInventoryRepository _foodInventoryRepository;
        public FoodInventoryService(IFoodInventoryRepository foodInventoryRepository)
        {
            _foodInventoryRepository = foodInventoryRepository;
        }
        public byte[] GenerateSalt(int length)
        {
            byte[] salt = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public async Task<FoodStoreEntity> getFoodInventoryById(FoodInventoryLoginRequestModel foodInventory)
        {
            return await _foodInventoryRepository.getFoodInventoryById(foodInventory);
        }

        public byte[] HashPassword(string password, byte[] salt, int iterations = 10000)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return pbkdf2.GetBytes(32);
            }
        }

        public bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }
}
