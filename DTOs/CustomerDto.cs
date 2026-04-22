namespace VehicleManagementAPI.DTOs
{
    public class CustomerDto
    {
        public int User_Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public List<VehicleDto> Vehicles { get; set; }
        public int Booking_Count { get; set; }
        public decimal Total_Spent { get; set; }
        public decimal Pending_Credit { get; set; }
    }

    public class CustomerRegisterDto : UserRegisterDto
    {
        public List<VehicleDto> Vehicles { get; set; }
    }
}
