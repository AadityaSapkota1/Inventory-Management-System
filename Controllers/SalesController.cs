using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<SalesInvoiceDto>> CreateSalesInvoice(SalesInvoiceDto invoiceDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = new SalesInvoice
                {
                    Sales_Date = DateTime.UtcNow,
                    User_Id = invoiceDto.User_Id,
                    Payment_Status = invoiceDto.Payment_Status ?? "Paid",
                    SalesItems = new List<SalesItem>()
                };

                decimal total = 0;
                foreach (var itemDto in invoiceDto.Items)
                {
                    var part = await _context.Parts.FindAsync(itemDto.Part_Id);
                    if (part == null)
                    {
                        return BadRequest($"Part with ID {itemDto.Part_Id} not found.");
                    }

                    if (part.Stock < itemDto.Quantity)
                    {
                        return BadRequest($"Insufficient stock for part: {part.Part_Name}. Available: {part.Stock}");
                    }

                    // Reduce stock
                    part.Stock -= itemDto.Quantity;

                    // Automatically notify admin if low stock (<10) - Feature 15
                    if (part.Stock < 10)
                    {
                        // Logic for notification could be added here (e.g. to Notifications table)
                        _context.Notifications.Add(new Notification
                        {
                            Notification_Message = $"Low stock alert for {part.Part_Name}. Current stock: {part.Stock}",
                            Notification_Time = DateTime.UtcNow,
                            User_Id = 1 // Assuming 1 is the Admin ID for now
                        });
                    }

                    var itemPrice = part.Part_Price; // Use current part price
                    total += itemPrice * itemDto.Quantity;

                    invoice.SalesItems.Add(new SalesItem
                    {
                        Part_Id = itemDto.Part_Id,
                        Quantity = itemDto.Quantity,
                        Price = itemPrice
                    });
                }

                // Loyalty Program: 10% discount if spend > 5000 (Feature 16)
                if (total > 5000)
                {
                    total *= 0.9m;
                }

                invoice.Total_Amount = total;

                _context.SalesInvoices.Add(invoice);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                invoiceDto.Sales_Invoice_ID = invoice.Sales_Invoice_ID;
                invoiceDto.Sales_Date = invoice.Sales_Date;
                invoiceDto.Total_Amount = invoice.Total_Amount;

                return Ok(invoiceDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("customer/{userId}")]
        public async Task<ActionResult<IEnumerable<SalesInvoiceDto>>> GetCustomerHistory(int userId)
        {
            return await _context.SalesInvoices
                .Where(si => si.User_Id == userId)
                .Include(si => si.SalesItems)
                .Select(si => new SalesInvoiceDto
                {
                    Sales_Invoice_ID = si.Sales_Invoice_ID,
                    Sales_Date = si.Sales_Date,
                    Total_Amount = si.Total_Amount,
                    Payment_Status = si.Payment_Status,
                    User_Id = si.User_Id,
                    Items = si.SalesItems.Select(item => new SalesItemDto
                    {
                        Part_Id = item.Part_Id,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList()
                })
                .ToListAsync();
        }

        [HttpPost("send-invoice-email/{invoiceId}")]
        public async Task<IActionResult> SendInvoiceEmail(int invoiceId)
        {
            var invoice = await _context.SalesInvoices
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.Sales_Invoice_ID == invoiceId);

            if (invoice == null) return NotFound("Invoice not found.");

            // Mock email sending logic
            var customerEmail = invoice.User.Email;
            var subject = $"Invoice for your recent purchase - #{invoice.Sales_Invoice_ID}";
            var body = $"Dear {invoice.User.FullName},\n\nThank you for your purchase. Your total amount is {invoice.Total_Amount}.";

            // In a real application, you would use an SMTP client or an email service like SendGrid
            Console.WriteLine($"Sending email to {customerEmail}...");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Body: {body}");

            return Ok(new { Message = $"Invoice email sent successfully to {customerEmail}." });
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesInvoiceDto>>> GetAllSales()
        {
            return await _context.SalesInvoices
                .Include(si => si.User)
                .Include(si => si.SalesItems)
                .Select(si => new SalesInvoiceDto
                {
                    Sales_Invoice_ID = si.Sales_Invoice_ID,
                    Sales_Date = si.Sales_Date,
                    Total_Amount = si.Total_Amount,
                    Payment_Status = si.Payment_Status,
                    User_Id = si.User_Id,
                    Customer_Name = si.User.FullName,
                    Items = si.SalesItems.Select(item => new SalesItemDto
                    {
                        Part_Id = item.Part_Id,
                        Quantity = item.Quantity,
                        Price = item.Price
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}
