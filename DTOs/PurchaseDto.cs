namespace VehicleManagementAPI.DTOs
{
    public class PurchaseInvoiceDto
    {
        public int Purchase_Invoice_ID { get; set; }
        public DateTime Purchase_Date { get; set; }
        public decimal Total_Amount { get; set; }
        public string Payment_Status { get; set; }
        public int Vendor_Id { get; set; }
        public string Vendor_Name { get; set; }
        public List<PurchaseItemDto> Items { get; set; }
    }

    public class PurchaseItemDto
    {
        public int Part_Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
