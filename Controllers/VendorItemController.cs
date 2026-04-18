using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorItemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorItemDto>>> GetVendorItems()
        {
            return await _context.VendorItems
                .Include(v => v.Vendor)
                .Select(v => new VendorItemDto
                {
                    VendorItem_Id = v.VendorItem_Id,
                    Part_Name = v.Part_Name,
                    Part_Price = v.Part_Price,
                    Available = v.Available,
                    Vendor_Id = v.Vendor_Id,
                    Vendor_Name = v.Vendor.Vendor_Name,
                    Part_Id = v.Part_Id
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorItemDto>> GetVendorItem(int id)
        {
            var item = await _context.VendorItems
                .Include(v => v.Vendor)
                .FirstOrDefaultAsync(v => v.VendorItem_Id == id);

            if (item == null) return NotFound();

            return new VendorItemDto
            {
                VendorItem_Id = item.VendorItem_Id,
                Part_Name = item.Part_Name,
                Part_Price = item.Part_Price,
                Available = item.Available,
                Vendor_Id = item.Vendor_Id,
                Vendor_Name = item.Vendor.Vendor_Name,
                Part_Id = item.Part_Id
            };
        }

        [HttpPost]
        public async Task<ActionResult<VendorItemDto>> CreateVendorItem(VendorItemDto dto)
        {
            var item = new VendorItem
            {
                Part_Name = dto.Part_Name,
                Part_Price = dto.Part_Price,
                Available = dto.Available,
                Vendor_Id = dto.Vendor_Id,
                Part_Id = dto.Part_Id
            };

            _context.VendorItems.Add(item);
            await _context.SaveChangesAsync();

            dto.VendorItem_Id = item.VendorItem_Id;
            return CreatedAtAction(nameof(GetVendorItem), new { id = item.VendorItem_Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendorItem(int id, VendorItemDto dto)
        {
            var item = await _context.VendorItems.FindAsync(id);
            if (item == null) return NotFound();

            item.Part_Name = dto.Part_Name;
            item.Part_Price = dto.Part_Price;
            item.Available = dto.Available;
            item.Vendor_Id = dto.Vendor_Id;
            item.Part_Id = dto.Part_Id;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendorItem(int id)
        {
            var item = await _context.VendorItems.FindAsync(id);
            if (item == null) return NotFound();

            _context.VendorItems.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
