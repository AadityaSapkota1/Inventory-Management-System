using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementAPI.Models
{
    public class Part
    {
        [Key]
        public int Part_Id { get; set; }
        
        [Required]
        public string Part_Name { get; set; }
        
        [Required]
        public decimal Part_Price { get; set; }
        
        [Required]
        public int Stock { get; set; }

        public ICollection<VendorItem> VendorItems { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
        public ICollection<SalesItem> SalesItems { get; set; }
    }
}
