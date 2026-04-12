namespace VehicleManagementAPI.DTOs
{
    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string Password { get; set; }
        public string? User_Role { get; set; } // Admin, Staff, Customer
    }

    public class UserResponseDto
    {
        public int User_Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public string User_Role { get; set; }
    }
}
