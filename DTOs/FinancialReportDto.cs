namespace VehicleManagementAPI.DTOs
{
    public class FinancialReportDto
    {
        public string Period { get; set; }
        public decimal Sales { get; set; }
        public decimal Purchases { get; set; }
        public decimal Profit { get; set; }
    }
}
