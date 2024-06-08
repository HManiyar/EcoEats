using AutoMapper;
using coremvctest.Models;
using coremvctest.RequestModels;

namespace coremvctest.AutoMapper
{
    public class AutoMapper:Profile
    {
       public AutoMapper()
        {
            CreateMap<NGORequestModels, NGOEntity>();
            CreateMap<FoodInventoryRequestModel, FoodStoreEntity>();
        }
    }
}
