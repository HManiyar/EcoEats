using coremvctest.Data;
using coremvctest.IRepository;
using coremvctest.Models;
using coremvctest.RequestModels;
using Microsoft.EntityFrameworkCore;

namespace coremvctest.Repository
{
    public class NGORepository : INGORepository
    {
        private readonly ApplicationDbContext _db;
        public NGORepository(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<NGOEntity> getNGOById(NGOLoginRequestModel ngo)
        {
            return await _db.NGOs.SingleOrDefaultAsync(m =>
         m.NGOUserName == ngo.NGOUserName)??new NGOEntity();
        }
    }
}
