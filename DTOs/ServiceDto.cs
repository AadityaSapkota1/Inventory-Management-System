using System;
using System.Collections.Generic;

namespace VehicleManagementAPI.DTOs
{
    public class AppointmentBookDto
    {
        public DateTime Service_Date { get; set; }
        public string Service_Time { get; set; }
        public int Vehicle_Id { get; set; }
        public int User_Id { get; set; }
    }

    public class AppointmentResponseDto
    {
        public int Appointment_Id { get; set; }
        public DateTime Service_Date { get; set; }
        public string Service_Time { get; set; }
        public string Status { get; set; }
        public string Vehicle_Number { get; set; }
        public string Vehicle_Type { get; set; }
    }
}
