using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace coremvctest.Models
{
    public class NGOEntity
    {
        [Key]
        public int NGOId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public string ContactPerson { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }
        [JsonIgnore]
        public byte[]? HashPassword { get; set; }
        [JsonIgnore]
        public byte[]? Salt { get; set; }

        [Required]
        public string NGONumber { get; set; }

        //[Required]
        //public DateTime RegistrationDate { get; set; }

        public bool IsActive { get; set; }
        public string NGOUserName { get; set; }
    }
    public class RequestedFoods
    {
        public DateTime DeliveryTime { get; set; }
        public List<FoodRequest> medicines { get; set; }
    }

    public class FoodRequest
    {
        public string category { get; set; }
        public int quantity { get; set; }
    }
}
