using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementAPI.Models
{
    public class PurchaseInvoice
    {
        [Key]
        public int Purchase_Invoice_ID { get; set; }
        
        [Required]
        public DateTime Purchase_Date { get; set; }
        
        [Required]
        public decimal Total_Amount { get; set; }

        [Required]
        public string Payment_Status { get; set; } // Paid, Unpaid, Partial
        
        public int Vendor_Id { get; set; }
        
        [ForeignKey("Vendor_Id")]
        public Vendor Vendor { get; set; }

        public ICollection<PurchaseItem> PurchaseItems { get; set; }
    }

    public class PurchaseItem
    {
        [Key]
        public int PurchaseItem_Id { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        public int Purchase_Invoice_ID { get; set; }
        
        [ForeignKey("Purchase_Invoice_ID")]
        public PurchaseInvoice PurchaseInvoice { get; set; }
        
        public int Part_Id { get; set; }
        
        [ForeignKey("Part_Id")]
        public Part Part { get; set; }
    }
}
