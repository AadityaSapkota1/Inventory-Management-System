namespace VehicleManagementAPI.DTOs
{
    public class VendorItemDto
    {
        public int VendorItem_Id { get; set; }
        public string Part_Name { get; set; }
        public decimal Part_Price { get; set; }
        public bool Available { get; set; }
        public int Vendor_Id { get; set; }
        public string Vendor_Name { get; set; }
        public int Part_Id { get; set; }
    }
}
