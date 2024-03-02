using coremvctest.Data;
using Microsoft.AspNetCore.Mvc;

namespace coremvctest.Controllers
{
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _db;
        public LocationController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult GetLocations()
        {
            var locations = _db.Locations.ToList();  // Assuming _dbContext is your DbContext instance

            return Json(locations);
        }
    }
}
