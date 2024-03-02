using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using coremvctest.Data;
using coremvctest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace coremvctest.Controllers
{
    public class FoodInventoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        public FoodInventoryController(ApplicationDbContext db,IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }
        public IActionResult FoodInventoryDashboard()
        {
            return View();
        }
        [HttpGet]
        public IActionResult FoodInventorySignUp()
        {
            return View();
        }
        public IActionResult UpdateFood()
        {
            List<ExpireWarnings> warningMsgs = _db.ExpireWarnings.ToList();
            ViewBag.WarningMsgs = warningMsgs;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateFood(FoodsEntity foodsData, IFormFile ImageFileName)
            {
            try
            {
                foodsData.RemainingDays = 0;
                foodsData.ImageFileName = ImageFileName.FileName;

                // Assuming the medicine data is valid, save it in the database
                int? foodInventoryId = HttpContext.Session.GetInt32("FoodInventoryId");
                foodsData.FoodInventoryId = foodInventoryId.Value;
                _db.Foods.Add(foodsData);
                await _db.SaveChangesAsync(); // Save medicine data asynchronously

                // After successfully saving the medicine data, execute the second stored procedure
                var foods = await _db.Foods
                    .FromSqlRaw("CALL GetFoodsWithExpiryWarning")
                    .ToListAsync();

                // Continue with your code to upload the image to S3 if needed
                if (ImageFileName != null)
                {
                    var base64Image = await UploadImageToS3(ImageFileName);
                    // Do something with the base64 image, like saving it in the database or any other desired operation
                }

                return RedirectToAction("FoodInventoryDashboard");
            }
            catch (Exception ex)
            {
                // Handle any exceptions, for example, validation errors
                // You can add validation logic here and return a view with error messages
                return View("FoodInventoryDashboard", ex);
            }
        }

        public string HashPassword(string password)
        {
            // Generate a random salt
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            // Hash the password with the salt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            // Verify the password against the hashed password
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        [HttpPost]
        public async Task<IActionResult> FoodInventorySignUp(FoodStoreEntity foodStore)
        {
            //try
            //{
            if (ModelState.IsValid)
            {
                foodStore.Password = HashPassword(foodStore.Password);
                _db.FoodInventories.Add(foodStore);
                _db.SaveChanges();
                var location = _db.Locations.Where(location => location.Id == Convert.ToInt32(foodStore.Location)).FirstOrDefault();
                var message = "Hello admin! New Food Inventory: " + foodStore.StoreName + " has been registered with the following details:\n"
                    + "Store Name: " + foodStore.StoreName + "\n"
                    + "Location: " + location.Name + "\n"
                    + "Contact Person: " + foodStore.ContactPerson + "\n"
                    + "Email: " + foodStore.Email + "\n"
                    + "Phone: " + foodStore.Phone + "\n";
                var credentials =new BasicAWSCredentials(_configuration["AWS:AccessKeyId"], _configuration["AWS:SecretAccessKey"]);
                var client = new AmazonSimpleNotificationServiceClient(credentials, Amazon.RegionEndpoint.USEast1);
                var request = new PublishRequest()
                {
                    TopicArn = _configuration["AWS:SnsArnKey"],
                    Message = message,
                    Subject = "Registration of New Food Inventory!"
                };
                var resoponse = await client.PublishAsync(request);
                if (resoponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("FoodInventoryLogin");
                }
                else
                {
                    return RedirectToAction("FoodInventorySignUp");
                }
            }
            else
            {
                return RedirectToAction("FoodInventorySignUp");
            }
            //}
            //catch(Exception ex)
            //{

            //}
        }
        public IActionResult FoodInventoryLogin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult FoodInventoryLogin(FoodStoreEntity foodStore)
        {
            var existingFoodInventory = _db.FoodInventories.SingleOrDefault(m =>
         m.FoodInventoryUserName == foodStore.FoodInventoryUserName);

            if (existingFoodInventory != null && VerifyPassword(foodStore.Password, existingFoodInventory.Password))
            {
                HttpContext.Session.SetInt32("FoodInventoryId", existingFoodInventory.FoodInventoryId);
                return RedirectToAction("FoodInventoryDashboard");
            }
            else
            {
                // Authentication failed, return a view indicating login failure
                return View("FoodInventoryLogin");
            }
        }
        [HttpPost]
        private async Task<string> UploadImageToS3(IFormFile formFile)
        {
            var ms = new MemoryStream();
            var bucketName = "foodsimagesmrdsharebucket";
            var credentials =new BasicAWSCredentials(_configuration["AWS:AccessKeyId"], _configuration["AWS:SecretAccessKey"]);
            var client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast1);

            var isBucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(client, bucketName);
            if (!isBucketExists)
            {
                var bucketRequest = new PutBucketRequest()
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };
                try { 
                await client.PutBucketAsync(bucketRequest);
                }
                catch (AmazonS3Exception ex)
                {
                    // Handle the exception, log it, and possibly redirect to an error view
                    // Avoid passing 'ex' to the view
                    return ex.Message;
                }
            }

            using (ms = new MemoryStream())
            {
                await formFile.CopyToAsync(ms); // Use async version to read the form file into memory stream
                var objectRequest = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    //Key = $"{DateTime.Now:yyyy\\/MM\\/dd\\/hhmmss}-{formFile.FileName}",
                    Key = formFile.FileName,
                    InputStream = ms
                };
                var response = await client.PutObjectAsync(objectRequest);
            }

            // Generate and return the base64 representation of the image
            return Convert.ToBase64String(ms.ToArray());
        }

        [HttpGet]
        public async Task<IActionResult> GetImage(string fileName)
        {
            var bucketName = "foodsimagesmedshare";
            var credentials =new BasicAWSCredentials(_configuration["AWS:AccessKeyId"], _configuration["AWS:SecretAccessKey"]);
            var client = new AmazonS3Client(credentials, Amazon.RegionEndpoint.USEast1);
            var response = await client.GetObjectAsync(bucketName, fileName);

            return File(response.ResponseStream, response.Headers.ContentType);
        }
      

    }
}
