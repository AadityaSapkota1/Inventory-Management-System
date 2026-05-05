using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserRegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
            {
                return BadRequest("Username or Email already exists.");
            }

            var user = new User
            {
                Username = registerDto.Username,
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Contact = registerDto.Contact,
                Password = registerDto.Password,
                User_Role = "Customer"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new UserResponseDto
            {
                User_Id = user.User_Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Contact = user.Contact,
                User_Role = user.User_Role
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.Password == loginDto.Password);

            if (user == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            return Ok(new UserResponseDto
            {
                User_Id = user.User_Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Contact = user.Contact,
                User_Role = user.User_Role
            });
        }

        [HttpGet("profile/{id}")]
        public async Task<ActionResult<CustomerDto>> GetProfile(int id)
        {
            var user = await _context.Users
                .Include(u => u.Vehicles)
                .FirstOrDefaultAsync(u => u.User_Id == id);

            if (user == null) return NotFound("User not found.");

            return Ok(new CustomerDto
            {
                User_Id = user.User_Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Contact = user.Contact,
                Vehicles = user.Vehicles.Select(v => new VehicleDto
                {
                    Vehicle_Id = v.Vehicle_Id,
                    Vehicle_Number = v.Vehicle_Number,
                    Vehicle_Type = v.Vehicle_Type
                }).ToList()
            });
        }

        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, UserRegisterDto profileDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound("User not found.");

            user.FullName = profileDto.FullName;
            user.Email = profileDto.Email;
            user.Contact = profileDto.Contact;
            if (!string.IsNullOrEmpty(profileDto.Password))
            {
                user.Password = profileDto.Password;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("add-vehicle")]
        public async Task<ActionResult<VehicleDto>> AddVehicle(VehicleDto vehicleDto)
        {
            var vehicle = new Vehicle
            {
                Vehicle_Number = vehicleDto.Vehicle_Number,
                Vehicle_Type = vehicleDto.Vehicle_Type,
                User_Id = vehicleDto.User_Id
            };

            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();

            vehicleDto.Vehicle_Id = vehicle.Vehicle_Id;
            return Ok(vehicleDto);
        }

        [HttpDelete("remove-vehicle/{id}")]
        public async Task<IActionResult> RemoveVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null) return NotFound("Vehicle not found.");

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("appointments/{userId}")]
        public async Task<ActionResult<List<AppointmentResponseDto>>> GetAppointments(int userId)
        {
            var appointments = await _context.Appointments
                .Include(a => a.Vehicle)
                .Where(a => a.User_Id == userId)
                .OrderByDescending(a => a.Service_Date)
                .Select(a => new AppointmentResponseDto
                {
                    Appointment_Id = a.Appointment_Id,
                    Service_Date = a.Service_Date,
                    Service_Time = a.Service_Time,
                    Status = a.Status,
                    Vehicle_Number = a.Vehicle.Vehicle_Number,
                    Vehicle_Type = a.Vehicle.Vehicle_Type
                })
                .ToListAsync();

            return Ok(appointments);
        }

        [HttpPost("book-appointment")]
        public async Task<IActionResult> BookAppointment(AppointmentBookDto appointmentDto)
        {
            try
            {
                Console.WriteLine($"Booking attempt: User={appointmentDto.User_Id}, Vehicle={appointmentDto.Vehicle_Id}, Date={appointmentDto.Service_Date}");

                if (appointmentDto.Service_Date == default)
                {
                    return BadRequest(new { message = "Service date is required." });
                }

                if (appointmentDto.User_Id <= 0 || appointmentDto.Vehicle_Id <= 0)
                {
                    return BadRequest(new { message = "Invalid User or Vehicle selection." });
                }

                var appointment = new Appointment
                {
                    Service_Date = DateTime.SpecifyKind(appointmentDto.Service_Date, DateTimeKind.Utc),
                    Service_Time = appointmentDto.Service_Time ?? "09:00",
                    Vehicle_Id = appointmentDto.Vehicle_Id,
                    User_Id = appointmentDto.User_Id,
                    Status = "Pending"
                };

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Appointment booked successfully" });
            }
            catch (DbUpdateException dbex)
            {
                Console.WriteLine($"DB Update Error: {dbex.Message}");
                var inner = dbex.InnerException?.Message ?? "No inner exception";
                Console.WriteLine($"Inner: {inner}");
                return StatusCode(500, new { 
                    message = "Database update failed.", 
                    details = dbex.Message,
                    inner = inner
                });
            }
            catch (Npgsql.PostgresException pex)
            {
                Console.WriteLine($"Postgres Error: {pex.MessageText}");
                Console.WriteLine($"Constraint: {pex.ConstraintName}");
                Console.WriteLine($"Table: {pex.TableName}");
                return StatusCode(500, new { 
                    message = "Database constraint violation.", 
                    details = pex.MessageText,
                    constraint = pex.ConstraintName
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error booking appointment: {ex.Message}");
                var innerMessage = ex.InnerException?.Message ?? "No inner exception";
                Console.WriteLine($"Inner Exception: {innerMessage}");
                
                return StatusCode(500, new { 
                    message = "Database error occurred.", 
                    details = ex.Message, 
                    inner = innerMessage 
                });
            }
        }

        [HttpPost("request-part")]
        public async Task<IActionResult> RequestPart(PartRequestDto requestDto)
        {
            var request = new PartRequest
            {
                Part_Name = requestDto.Part_Name,
                Description = requestDto.Description,
                Request_Date = DateTime.UtcNow,
                User_Id = requestDto.User_Id
            };

            _context.PartRequests.Add(request);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Part request submitted successfully" });
        }

        [HttpPost("submit-review")]
        public async Task<IActionResult> SubmitReview(ServiceReviewDto reviewDto)
        {
            var review = new ServiceReview
            {
                Review_Text = reviewDto.Review_Text,
                Rating = reviewDto.Rating,
                Review_Date = DateTime.UtcNow,
                User_Id = reviewDto.User_Id
            };

            _context.ServiceReviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Review submitted successfully" });
        }

        [HttpGet("part-requests/{userId}")]
        public async Task<ActionResult<IEnumerable<PartRequestDto>>> GetPartRequests(int userId)
        {
            return await _context.PartRequests
                .Where(r => r.User_Id == userId)
                .Select(r => new PartRequestDto
                {
                    Request_Id = r.Request_Id,
                    Part_Name = r.Part_Name,
                    Description = r.Description,
                    Request_Date = r.Request_Date,
                    User_Id = r.User_Id
                })
                .ToListAsync();
        }

        [HttpGet("reviews/{userId}")]
        public async Task<ActionResult<IEnumerable<ServiceReviewDto>>> GetReviews(int userId)
        {
            return await _context.ServiceReviews
                .Where(r => r.User_Id == userId)
                .Select(r => new ServiceReviewDto
                {
                    Review_Id = r.Review_Id,
                    Review_Text = r.Review_Text,
                    Rating = r.Rating,
                    Review_Date = r.Review_Date,
                    User_Id = r.User_Id
                })
                .ToListAsync();
        }

        [HttpGet("notifications/{userId}")]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications(int userId)
        {
            return await _context.Notifications
                .Where(n => n.User_Id == userId)
                .OrderByDescending(n => n.Notification_Time)
                .ToListAsync();
        }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
