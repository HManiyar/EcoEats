using coremvctest.Models;
using coremvctest.RequestModels;

namespace coremvctest.IRepository
{
    public interface INGORepository
    {
        Task<NGOEntity> getNGOById(NGOLoginRequestModel foodStore);
    }
}
