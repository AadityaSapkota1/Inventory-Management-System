using System.ComponentModel.DataAnnotations;

namespace VehicleManagementAPI.Models
{
    public class Vendor
    {
        [Key]
        public int Vendor_Id { get; set; }
        
        [Required]
        public string Vendor_Name { get; set; }
        
        public string Contact { get; set; }
        public string Address { get; set; }

        public ICollection<VendorItem> VendorItems { get; set; }
        public ICollection<PurchaseInvoice> PurchaseInvoices { get; set; }
    }
}
