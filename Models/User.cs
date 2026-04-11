using System.ComponentModel.DataAnnotations;

namespace VehicleManagementAPI.Models
{
    public class User
    {
        [Key]
        public int User_Id { get; set; }
        
        [Required]
        public string User_Role { get; set; } // Admin, Staff, Customer
        
        [Required]
        public string Username { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        public string Contact { get; set; }
        
        [Required]
        public string Password { get; set; }

        // Navigation properties
        public ICollection<Vehicle> Vehicles { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<SalesInvoice> SalesInvoices { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}
