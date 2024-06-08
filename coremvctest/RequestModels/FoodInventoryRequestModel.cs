using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace coremvctest.RequestModels
{
    public class FoodInventoryRequestModel
    {
        public string? Password { get; set; }
        public string? StoreName { get; set; }
        public string? Location { get; set; }
        public string? ContactPerson { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        [Phone]
        public string? Phone { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? LicenseNumber { get; set; }
        public bool? IsActive { get; set; }
        public string? FoodInventoryUserName { get; set; }
        [JsonIgnore]
        public byte[]? HashPassword { get; set; }
        [JsonIgnore]
        public byte[]? Salt { get; set; }
    }
}
