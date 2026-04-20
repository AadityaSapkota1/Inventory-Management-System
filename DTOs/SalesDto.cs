namespace VehicleManagementAPI.DTOs
{
    public class SalesInvoiceDto
    {
        public int Sales_Invoice_ID { get; set; }
        public DateTime Sales_Date { get; set; }
        public decimal Total_Amount { get; set; }
        public string? Payment_Status { get; set; }
        public int User_Id { get; set; }
        public string? Customer_Name { get; set; }
        public List<SalesItemDto>? Items { get; set; }
    }

    public class SalesItemDto
    {
        public int Part_Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
