using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // Feature 1: Admin can manage staff registration and roles
        [HttpPost("register-staff")]
        public async Task<ActionResult<UserResponseDto>> RegisterStaff(UserRegisterDto staffDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == staffDto.Username || u.Email == staffDto.Email))
            {
                return BadRequest("Username or Email already exists.");
            }

            var staff = new User
            {
                Username = staffDto.Username,
                FullName = staffDto.FullName,
                Email = staffDto.Email,
                Contact = staffDto.Contact,
                Password = staffDto.Password, // In a real app, hash this!
                User_Role = "Staff"
            };

            _context.Users.Add(staff);
            await _context.SaveChangesAsync();

            return Ok(new UserResponseDto
            {
                User_Id = staff.User_Id,
                Username = staff.Username,
                FullName = staff.FullName,
                Email = staff.Email,
                Contact = staff.Contact,
                User_Role = staff.User_Role
            });
        }

        [HttpGet("staff")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllStaff()
        {
            var staffList = await _context.Users
                .Where(u => u.User_Role == "Staff")
                .Select(u => new UserResponseDto
                {
                    User_Id = u.User_Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Contact = u.Contact,
                    User_Role = u.User_Role
                })
                .ToListAsync();

            return Ok(staffList);
        }

        [HttpPut("update-staff/{id}")]
        public async Task<IActionResult> UpdateStaff(int id, UserRegisterDto staffDto)
        {
            var staff = await _context.Users.FindAsync(id);
            if (staff == null || staff.User_Role != "Staff")
            {
                return NotFound("Staff not found.");
            }

            staff.FullName = staffDto.FullName;
            staff.Contact = staffDto.Contact;
            staff.Email = staffDto.Email;
            // Only update password if provided
            if (!string.IsNullOrEmpty(staffDto.Password))
            {
                staff.Password = staffDto.Password;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Prevent self-deletion if we had auth, but for now just delete
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("delete-staff/{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var staff = await _context.Users.FindAsync(id);
            if (staff == null || staff.User_Role != "Staff")
            {
                return NotFound("Staff not found.");
            }

            _context.Users.Remove(staff);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("financial-report")]
        public async Task<ActionResult<IEnumerable<FinancialReportDto>>> GetFinancialReport()
        {
            var today = DateTime.UtcNow.Date;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);

            // Daily
            var dailySales = await _context.SalesInvoices
                .Where(s => s.Sales_Date >= today)
                .SumAsync(s => s.Total_Amount);
            var dailyPurchases = await _context.PurchaseInvoices
                .Where(p => p.Purchase_Date >= today)
                .SumAsync(p => p.Total_Amount);

            // Monthly
            var monthlySales = await _context.SalesInvoices
                .Where(s => s.Sales_Date >= startOfMonth)
                .SumAsync(s => s.Total_Amount);
            var monthlyPurchases = await _context.PurchaseInvoices
                .Where(p => p.Purchase_Date >= startOfMonth)
                .SumAsync(p => p.Total_Amount);

            // Yearly
            var yearlySales = await _context.SalesInvoices
                .Where(s => s.Sales_Date >= startOfYear)
                .SumAsync(s => s.Total_Amount);
            var yearlyPurchases = await _context.PurchaseInvoices
                .Where(p => p.Purchase_Date >= startOfYear)
                .SumAsync(p => p.Total_Amount);

            var reports = new List<FinancialReportDto>
            {
                new FinancialReportDto { Period = "Daily", Sales = dailySales, Purchases = dailyPurchases, Profit = dailySales - dailyPurchases },
                new FinancialReportDto { Period = "Monthly", Sales = monthlySales, Purchases = monthlyPurchases, Profit = monthlySales - monthlyPurchases },
                new FinancialReportDto { Period = "Yearly", Sales = yearlySales, Purchases = yearlyPurchases, Profit = yearlySales - yearlyPurchases }
            };

            return Ok(reports);
        }
    }
}
