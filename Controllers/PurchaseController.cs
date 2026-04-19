using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PurchaseController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseInvoiceDto>> CreatePurchaseInvoice(PurchaseInvoiceDto invoiceDto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var invoice = new PurchaseInvoice
                {
                    Purchase_Date = DateTime.UtcNow,
                    Vendor_Id = invoiceDto.Vendor_Id,
                    Total_Amount = invoiceDto.Items.Sum(i => i.Quantity * i.Price),
                    Payment_Status = invoiceDto.Payment_Status ?? "Unpaid",
                    PurchaseItems = new List<PurchaseItem>()
                };

                foreach (var itemDto in invoiceDto.Items)
                {
                    var part = await _context.Parts.FindAsync(itemDto.Part_Id);
                    if (part == null)
                    {
                        return BadRequest($"Part with ID {itemDto.Part_Id} not found.");
                    }

                    // Update stock
                    part.Stock += itemDto.Quantity;

                    invoice.PurchaseItems.Add(new PurchaseItem
                    {
                        Part_Id = itemDto.Part_Id,
                        Quantity = itemDto.Quantity,
                        Price = itemDto.Price
                    });
                }

                _context.PurchaseInvoices.Add(invoice);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                invoiceDto.Purchase_Invoice_ID = invoice.Purchase_Invoice_ID;
                invoiceDto.Purchase_Date = invoice.Purchase_Date;
                invoiceDto.Total_Amount = invoice.Total_Amount;

                return Ok(invoiceDto);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseInvoiceDto>>> GetPurchaseInvoices()
        {
            return await _context.PurchaseInvoices
                .Include(pi => pi.Vendor)
                .Include(pi => pi.PurchaseItems)
                .Select(pi => new PurchaseInvoiceDto
                {
                    Purchase_Invoice_ID = pi.Purchase_Invoice_ID,
                    Purchase_Date = pi.Purchase_Date,
                    Total_Amount = pi.Total_Amount,
                    Payment_Status = pi.Payment_Status,
                    Vendor_Id = pi.Vendor_Id,
                    Vendor_Name = pi.Vendor != null ? pi.Vendor.Vendor_Name : "Unknown",
                    Items = pi.PurchaseItems.Select(item => new PurchaseItemDto
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
