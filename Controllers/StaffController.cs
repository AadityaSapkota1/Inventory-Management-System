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
    }
}
