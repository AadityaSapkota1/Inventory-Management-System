using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementAPI.Models
{
    public class Appointment
    {
        [Key]
        public int Appointment_Id { get; set; }
        
        [Required]
        public DateTime Service_Date { get; set; }
        
        [Required]
        public string Service_Time { get; set; }
        
        [Required]
        public string Status { get; set; } // Pending, Approved, Completed, Cancelled
        
        public int User_Id { get; set; }
        
        [ForeignKey("User_Id")]
        public User User { get; set; }
        
        public int Vehicle_Id { get; set; }
        
        [ForeignKey("Vehicle_Id")]
        public Vehicle Vehicle { get; set; }
    }
}
