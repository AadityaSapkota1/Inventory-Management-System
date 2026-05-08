using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        // Feature 6: Staff can register new customers with vehicle details
        [HttpPost("register-customer")]
        public async Task<ActionResult<CustomerDto>> RegisterCustomer(CustomerRegisterDto customerDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == customerDto.Username || u.Email == customerDto.Email))
                {
                    return BadRequest("Username or Email already exists.");
                }

                var customer = new User
                {
                    Username = customerDto.Username,
                    FullName = customerDto.FullName,
                    Email = customerDto.Email,
                    Contact = customerDto.Contact,
                    Password = customerDto.Password,
                    User_Role = "Customer"
                };

                _context.Users.Add(customer);
                await _context.SaveChangesAsync();

                if (customerDto.Vehicles != null && customerDto.Vehicles.Any())
                {
                    foreach (var vDto in customerDto.Vehicles)
                    {
                        _context.Vehicles.Add(new Vehicle
                        {
                            Vehicle_Number = vDto.Vehicle_Number,
                            Vehicle_Type = vDto.Vehicle_Type,
                            User_Id = customer.User_Id
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new CustomerDto
                {
                    User_Id = customer.User_Id,
                    Username = customer.Username,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    Contact = customer.Contact,
                    Vehicles = customerDto.Vehicles
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Feature 7: Staff can search customers by vehicle number, phone, ID, or name
        [HttpGet("search-customers")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> SearchCustomers([FromQuery] string query)
        {
            if (string.IsNullOrEmpty(query)) return BadRequest("Query parameter is required.");

            var customers = await _context.Users
                .Include(u => u.Vehicles)
                .Where(u => u.User_Role == "Customer" && (
                    u.FullName.Contains(query) ||
                    u.Contact.Contains(query) ||
                    u.User_Id.ToString() == query ||
                    u.Vehicles.Any(v => v.Vehicle_Number.Contains(query))
                ))
                .Select(u => new CustomerDto
                {
                    User_Id = u.User_Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Contact = u.Contact,
                    Vehicles = u.Vehicles.Select(v => new VehicleDto
                    {
                        Vehicle_Id = v.Vehicle_Id,
                        Vehicle_Number = v.Vehicle_Number,
                        Vehicle_Type = v.Vehicle_Type,
                        User_Id = v.User_Id
                    }).ToList()
                })
                .ToListAsync();

            return Ok(customers);
        }

        // Feature 8: Staff can view customer details, history, and vehicle info
        [HttpGet("customer-details/{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerDetails(int id)
        {
            var customer = await _context.Users
                .Include(u => u.Vehicles)
                .Include(u => u.Appointments)
                .Include(u => u.SalesInvoices)
                .FirstOrDefaultAsync(u => u.User_Id == id && u.User_Role == "Customer");

            if (customer == null) return NotFound("Customer not found.");

            // Here we could include more details about history if needed, but for now basic info + vehicles
            return Ok(new CustomerDto
            {
                User_Id = customer.User_Id,
                Username = customer.Username,
                FullName = customer.FullName,
                Email = customer.Email,
                Contact = customer.Contact,
                Vehicles = customer.Vehicles.Select(v => new VehicleDto
                {
                    Vehicle_Id = v.Vehicle_Id,
                    Vehicle_Number = v.Vehicle_Number,
                    Vehicle_Type = v.Vehicle_Type,
                    User_Id = v.User_Id
                }).ToList()
            });
        }

        [HttpGet("all-part-requests")]
        public async Task<ActionResult<IEnumerable<PartRequestDto>>> GetAllPartRequests()
        {
            return await _context.PartRequests
                .Include(r => r.User)
                .Select(r => new PartRequestDto
                {
                    Request_Id = r.Request_Id,
                    Part_Name = r.Part_Name,
                    Description = r.Description,
                    Request_Date = r.Request_Date,
                    User_Id = r.User_Id,
                    Customer_Name = r.User.FullName
                })
                .ToListAsync();
        }

        [HttpGet("all-reviews")]
        public async Task<ActionResult<IEnumerable<ServiceReviewDto>>> GetAllReviews()
        {
            return await _context.ServiceReviews
                .Include(r => r.User)
                .Select(r => new ServiceReviewDto
                {
                    Review_Id = r.Review_Id,
                    Review_Text = r.Review_Text,
                    Rating = r.Rating,
                    Review_Date = r.Review_Date,
                    User_Id = r.User_Id,
                    Customer_Name = r.User.FullName
                })
                .ToListAsync();
        }

        [HttpGet("customers")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
        {
            var customers = await _context.Users
                .Include(u => u.Vehicles)
                .Include(u => u.Appointments)
                .Include(u => u.SalesInvoices)
                .Where(u => u.User_Role == "Customer")
                .Select(u => new CustomerDto
                {
                    User_Id = u.User_Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Contact = u.Contact,
                    Vehicles = u.Vehicles.Select(v => new VehicleDto
                    {
                        Vehicle_Id = v.Vehicle_Id,
                        Vehicle_Number = v.Vehicle_Number,
                        Vehicle_Type = v.Vehicle_Type,
                        User_Id = v.User_Id
                    }).ToList(),
                    Booking_Count = u.Appointments.Count,
                    Total_Spent = u.SalesInvoices.Sum(s => s.Total_Amount),
                    Pending_Credit = u.SalesInvoices.Where(s => s.Payment_Status != "Paid").Sum(s => s.Total_Amount)
                })
                .ToListAsync();

            return Ok(customers);
        }

        // Feature 9: Staff can generate customer-related reports
        [HttpGet("reports/regular-customers")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetRegularCustomers()
        {
            var customers = await _context.Users
                .Include(u => u.Appointments)
                .Where(u => u.User_Role == "Customer")
                .Select(u => new CustomerDto
                {
                    User_Id = u.User_Id,
                    FullName = u.FullName,
                    Booking_Count = u.Appointments.Count
                })
                .OrderByDescending(u => u.Booking_Count)
                .Take(10)
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("reports/high-spenders")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetHighSpenders()
        {
            var customers = await _context.Users
                .Include(u => u.SalesInvoices)
                .Where(u => u.User_Role == "Customer")
                .Select(u => new CustomerDto
                {
                    User_Id = u.User_Id,
                    FullName = u.FullName,
                    Total_Spent = u.SalesInvoices.Sum(s => s.Total_Amount)
                })
                .OrderByDescending(u => u.Total_Spent)
                .Take(10)
                .ToListAsync();

            return Ok(customers);
        }

        [HttpGet("reports/pending-credits")]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetPendingCredits()
        {
            var customers = await _context.Users
                .Include(u => u.SalesInvoices)
                .Where(u => u.User_Role == "Customer")
                .Select(u => new CustomerDto
                {
                    User_Id = u.User_Id,
                    FullName = u.FullName,
                    Pending_Credit = u.SalesInvoices.Where(s => s.Payment_Status != "Paid").Sum(s => s.Total_Amount)
                })
                .Where(u => u.Pending_Credit > 0)
                .OrderByDescending(u => u.Pending_Credit)
                .ToListAsync();

            return Ok(customers);
        }

        // Feature 15: Overdue credit reminders
        [HttpPost("send-overdue-reminders")]
        public async Task<IActionResult> SendOverdueReminders()
        {
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            var overdueInvoices = await _context.SalesInvoices
                .Include(i => i.User)
                .Where(i => i.Payment_Status != "Paid" && i.Sales_Date < oneMonthAgo)
                .ToListAsync();

            foreach (var invoice in overdueInvoices)
            {
                // Mock email sending
                Console.WriteLine($"Sending overdue reminder to {invoice.User.Email} for invoice #{invoice.Sales_Invoice_ID} (Date: {invoice.Sales_Date})");
                
                // Add notification to system
                _context.Notifications.Add(new Notification
                {
                    Notification_Message = $"Overdue payment reminder for invoice #{invoice.Sales_Invoice_ID}. Amount: {invoice.Total_Amount}",
                    Notification_Time = DateTime.UtcNow,
                    User_Id = invoice.User_Id
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { Message = $"{overdueInvoices.Count} overdue reminders processed." });
        }

        [HttpGet("appointments")]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAllAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Vehicle)
                .Include(a => a.User)
                .OrderByDescending(a => a.Service_Date)
                .Select(a => new AppointmentResponseDto
                {
                    Appointment_Id = a.Appointment_Id,
                    Service_Date = a.Service_Date,
                    Service_Time = a.Service_Time,
                    Status = a.Status,
                    Vehicle_Number = a.Vehicle.Vehicle_Number,
                    Vehicle_Type = a.Vehicle.Vehicle_Type,
                    Customer_Name = a.User.FullName,
                    Contact = a.User.Contact
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpPut("appointments/{id}/status")]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return NotFound();

            appointment.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
