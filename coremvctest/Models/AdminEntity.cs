using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace coremvctest.Models
{
    public class AdminEntity
    {
    }
    public class RequestedFoodsByNGO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int NGOId { get; set; }

        [ForeignKey("NGOId")]
        public virtual NGOEntity NGO { get; set; }

        public DateTime DeliveryTime { get; set; }

        [Required]
        [StringLength(255)]
        public string Category { get; set; }

        public int Quantity { get; set; }
    }
    public class RequestedInquiryFoodsResult
    {
        public string? Category { get; set; }
        //public int TotalRequestedQuantity { get; set; }

        // Properties representing columns from RequestedFoodsByNGO
        public int Id { get; set; }
        public int NGOId { get; set; }
        public DateTime DeliveryTime { get; set; }
        public int Quantity { get; set; }
        public string? NGOName { get; set; }
        public string? LocationName { get; set; }
        public string? ContactPerson { get; set; }
        public string? Phone { get; set; }
    }
}
