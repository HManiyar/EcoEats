using Amazon.SimpleNotificationService.Model;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace coremvctest.Models
{
    public class FoodStoreEntity
    {
        [Key]
        public int FoodInventoryId { get; set; }
        public string Password { get; set; }
        public string StoreName { get; set; }
        public string Location { get; set; }
        public string ContactPerson { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string LicenseNumber { get; set; }
        public bool IsActive { get; set; }
        public string FoodInventoryUserName { get; set; }
    }

    public class FoodsEntity
    {
        [Key]
        public int FoodId { get; set; }

        [Required]
        [MaxLength(255)]
        public string FoodName { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(50)]
        public string LotNumber { get; set; }

        [Required]
        [MaxLength(255)]
        public string StorageConditions { get; set; }

        [Required]
        [MaxLength(255)]
        public string Packaging { get; set; }

        [Required]
        [MaxLength(255)]
        public string ManufacturerInformation { get; set; }

        //[Required]
        //public Condition Condition { get; set; } // Create an enum for Condition

        public string AdditionalNotes { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImageFileName { get; set; }
        
        [ForeignKey("FoodInventoryId")]
        public int FoodInventoryId { get; set; }
        //[ForeignKey("ExpiryWarningId")]
        public int? ExpiryWarningId { get; set; }  // Foreign key property

        //public ExpireWarnings ExpiryWarning { get; set; }
        //[DataType(DataType.Date)]  
        public DateTime? CustomExpiryDate { get; set; }
        public int? RemainingDays { get; set; }
    }

    public enum Condition
    {
        New,
        Unopened,
        OriginalPackaging
    }

    public class ExpireWarnings
    {
        [Key]
        public int ExpiryWarningId { get; set; }
        public string ExpiryWarningName { get; set; }
    }
  
}
