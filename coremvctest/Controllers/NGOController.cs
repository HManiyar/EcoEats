using Amazon.Runtime;
using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using coremvctest.Models;
using Microsoft.AspNetCore.Mvc;
using coremvctest.Data;
using System.Net;
using MySqlConnector;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Accord.MachineLearning;
using System.Security.Cryptography;
using coremvctest.RequestModels;
using AutoMapper;
using coremvctest.IService;
using coremvctest.Utility.Content;

namespace coremvctest.Controllers
{
    public class NGOController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly INGOService _ngoService;
        private readonly IAuthenticationService _authenticationService;
        public NGOController(ApplicationDbContext db,IConfiguration configuration, IMapper mapper,INGOService ngoService,IAuthenticationService authenticationService)
        {
            _db = db;
            _configuration = configuration;
            _mapper = mapper;
            _ngoService = ngoService;
            _authenticationService = authenticationService;
        }
        public IActionResult NGODashboard()
        {
            return View();
        }
        public IActionResult NGOSignUp()
        {
            return View();
        }
        public IActionResult NGOLogin()
        {
            return View();
        }
        public IActionResult RequestFoodVIew()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NGOLogin(NGOLoginRequestModel ngo)
        {
            NGOEntity existingNGO = await _ngoService.getNGOById(ngo);

            if (existingNGO.NGOUserName != null && VerifyPassword(ngo.Password ?? String.Empty, existingNGO.Salt ?? new byte[0], existingNGO.HashPassword ?? new byte[0]))           
            {
                HttpContext.Session.SetInt32("NGOId", existingNGO.NGOId);
                HttpContext.Session.SetString("NGOLocation", existingNGO.Location);
                string token = _authenticationService.GenerateTokenForNGO(existingNGO);
                if (token == UserMessages.invalidCredentials)
                    return View("NGOLogin");
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(120)
                };
                Response.Cookies.Append("AuthToken", token, cookieOptions);
                return RedirectToAction("NGODashboard");
            }
            else
            {
                return View("NGOLogin");
            }
        }
        public bool VerifyPassword(string password, byte[] salt, byte[] hash, int iterations = 10000)
        {

            byte[] testHash = _ngoService.HashPassword(password, salt, iterations);
            return _ngoService.SlowEquals(hash,testHash);
        }
        [HttpPost]
        public async Task<IActionResult> NGOSignUp(NGORequestModels ngoData)
        {
            if (ModelState.IsValid)
            {
                byte[] salt = _ngoService.GenerateSalt(16);
                ngoData.Salt = salt;
                ngoData.HashPassword = _ngoService.HashPassword(ngoData.Password ?? String.Empty, salt); ;
                NGOEntity ngo = _mapper.Map<NGOEntity>(ngoData);
                ngo.IsActive = false;
                _db.NGOs.Add(ngo);
                _db.SaveChanges();
                var message = "Hello admin! New NGO: " + ngo.Name + " has been registered with the following details:\n"
                    + "NGO Name: " + ngo.Name + "\n"
                    + "Location: " + ngo.Location + "\n"
                    + "Contact Person: " + ngo.ContactPerson + "\n"
                    + "Email: " + ngo.Email + "\n"
                    + "Phone: " + ngo.Phone + "\n";
                var credentials = new BasicAWSCredentials(_configuration["AWS:AccessKeyId"], _configuration["AWS:SecretAccessKey"]);
                var client = new AmazonSimpleNotificationServiceClient(credentials, Amazon.RegionEndpoint.USEast1);
                var request = new PublishRequest()
                {
                    TopicArn = _configuration["AWS:SnsArnKey"],
                    Message = message,
                    Subject = "Registration of New NGO!"
                };
                var resoponse = await client.PublishAsync(request);
                if (resoponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("NGOLogin");
                }
                else
                {
                    return RedirectToAction("NGOSignUp");
                }
            }
            else
            {
                return RedirectToAction("NGOSignUp");
            }
           
        }
        [HttpPost]
        public IActionResult UpdateRequestedFood([FromBody] RequestedFoods requestData)
        {
            try
            {
                int? ngoId = HttpContext.Session.GetInt32("NGOId");
                var mySqlDateTime = new MySqlDateTime(requestData.DeliveryTime);
                var foodsJson = JsonConvert.SerializeObject(requestData);
                _db.Database.ExecuteSqlRaw("CALL InsertOrUpdateRequestedFoodsByNGO(@p_NGOId, @p_DeliveryTime, @p_RequestData)",
                    parameters: new[] {
                new MySqlParameter("@p_NGOId", MySqlDbType.Int32) { Value = ngoId.Value  },
                new MySqlParameter("@p_DeliveryTime", MySqlDbType.DateTime) { Value = mySqlDateTime },
                new MySqlParameter("@p_RequestData", MySqlDbType.JSON) { Value = foodsJson }
                    });

                return Json("Success");
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                return Json($"Error: {ex.Message}");
            }
        }
        public IActionResult GetFoodOptions()
        {
            string ngolocation = HttpContext.Session.GetString("NGOLocation");
            var locationData = _db.Locations.FirstOrDefault(locationId => locationId.Id == Convert.ToInt32(ngolocation));

            var nearestLocationsWithDistances = FindNearest(locationData.Name, 4);

            var foodsWithDistances = nearestLocationsWithDistances
                .SelectMany(locWithDistance => _db.Foods
                    .Where(food => Convert.ToInt32(food.Location) == locWithDistance.Location.Id)
                    .Select(foodEntity => new FoodEntityWithDistance
                    {
                        FoodEntity = foodEntity,
                        LocationName = locWithDistance.Location.Name,
                        //Distance = HaversineDistance(
                        //    new double[] { Convert.ToDouble(locationData.latitude), Convert.ToDouble(locationData.longitude) },
                        //    new double[] { Convert.ToDouble(locWithDistance.Location.latitude), Convert.ToDouble(locWithDistance.Location.longitude) }
                        //)
                    })
                )
                .ToList();

            return Json(foodsWithDistances);
        }

        public List<LocationWithDistance> FindNearest(string targetLocationName, int k)
        {
            var locationList = _db.Locations.ToArray();
            var coordinates = _db.Locations.Select(loc => new double[] { Convert.ToDouble(loc.latitude), Convert.ToDouble(loc.longitude) }).ToArray();

            var outputs = Enumerable.Range(0, locationList.Length).ToArray();

            var knn = new KNearestNeighbors(k: k);
            knn.Learn(coordinates, outputs);

            var referenceIndex = Array.FindIndex(locationList, loc => loc.Name == targetLocationName);
            var referenceCoordinates = coordinates[referenceIndex];

            int[] nearestIndices;
            knn.GetNearestNeighbors(referenceCoordinates, out nearestIndices);

            nearestIndices = nearestIndices.Distinct().ToArray();

            var nearestLocationsWithDistances = nearestIndices
                .Select(index => new LocationWithDistance
                {
                    Location = locationList[index],
                    Distance = HaversineDistance(referenceCoordinates, coordinates[index])
                })
                .ToList();

            return nearestLocationsWithDistances;
        }

        public double HaversineDistance(double[] coords1, double[] coords2)
        {
            const double R = 6378.1; // Earth radius in kilometers

            double dLat = DegreesToRadians(coords2[0] - coords1[0]);
            double dLon = DegreesToRadians(coords2[1] - coords1[1]);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(coords1[0])) * Math.Cos(DegreesToRadians(coords2[0])) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        public double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

    }
}
