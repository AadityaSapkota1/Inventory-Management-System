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
        public string Customer_Name { get; set; }
        public string Contact { get; set; }
    }

    public class PartRequestDto
    {
        public int Request_Id { get; set; }
        public string Part_Name { get; set; }
        public string Description { get; set; }
        public DateTime Request_Date { get; set; }
        public int User_Id { get; set; }
        public string? Customer_Name { get; set; }
    }

    public class ServiceReviewDto
    {
        public int Review_Id { get; set; }
        public string Review_Text { get; set; }
        public int Rating { get; set; }
        public DateTime Review_Date { get; set; }
        public int User_Id { get; set; }
        public string? Customer_Name { get; set; }
    }
}
