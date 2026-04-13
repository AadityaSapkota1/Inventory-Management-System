using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleManagementAPI.Models
{
    public class Vehicle
    {
        [Key]
        public int Vehicle_Id { get; set; }
        
        [Required]
        public string Vehicle_Number { get; set; }
        
        [Required]
        public string Vehicle_Type { get; set; }
        
        public int User_Id { get; set; }
        
        [ForeignKey("User_Id")]
        public User User { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
