using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementAPI.Models
{
    public class SalesInvoice
    {
        [Key]
        public int Sales_Invoice_ID { get; set; }
        
        [Required]
        public DateTime Sales_Date { get; set; }
        
        [Required]
        public decimal Total_Amount { get; set; }
        
        [Required]
        public string Payment_Status { get; set; } // Paid, Unpaid, Partial
        
        public int User_Id { get; set; }
        
        [ForeignKey("User_Id")]
        public User User { get; set; }
        
        public ICollection<SalesItem> SalesItems { get; set; }
    }

    public class VendorItem
    {
        [Key]
        public int VendorItem_Id { get; set; }

        [Required]
        public string Part_Name { get; set; }

        [Required]
        public decimal Part_Price { get; set; }

        [Required]
        public bool Available { get; set; }

        public int Vendor_Id { get; set; }

        [ForeignKey("Vendor_Id")]
        public Vendor Vendor { get; set; }

        public int Part_Id { get; set; }

        [ForeignKey("Part_Id")]
        public Part Part { get; set; }
    }

    public class SalesItem
    {
        [Key]
        public int SalesItem_Id { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        public int Sales_Invoice_ID { get; set; }
        
        [ForeignKey("Sales_Invoice_ID")]
        public SalesInvoice SalesInvoice { get; set; }
        
        public int Part_Id { get; set; }
        
        [ForeignKey("Part_Id")]
        public Part Part { get; set; }
    }

    public class Notification
    {
        [Key]
        public int Notification_Id { get; set; }
        
        [Required]
        public string Notification_Message { get; set; }
        
        [Required]
        public DateTime Notification_Time { get; set; }
        
        public int User_Id { get; set; }
        
        [ForeignKey("User_Id")]
        public User User { get; set; }
    }

    public class PartRequest
    {
        [Key]
        public int Request_Id { get; set; }
        
        [Required]
        public string Part_Name { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public DateTime Request_Date { get; set; }
        
        public int User_Id { get; set; }
        
        [ForeignKey("User_Id")]
        public User User { get; set; }
    }

    public class ServiceReview
    {
        [Key]
        public int Review_Id { get; set; }
        
        [Required]
        public string Review_Text { get; set; }
        
        [Required]
        public int Rating { get; set; } // 1-5
        
        [Required]
        public DateTime Review_Date { get; set; }
        
        public int User_Id { get; set; }
        
        [ForeignKey("User_Id")]
        public User User { get; set; }
    }
}
