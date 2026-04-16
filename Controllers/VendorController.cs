using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendorDto>>> GetVendors()
        {
            return await _context.Vendors
                .Select(v => new VendorDto
                {
                    Vendor_Id = v.Vendor_Id,
                    Vendor_Name = v.Vendor_Name,
                    Contact = v.Contact,
                    Address = v.Address
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendorDto>> GetVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();

            return new VendorDto
            {
                Vendor_Id = vendor.Vendor_Id,
                Vendor_Name = vendor.Vendor_Name,
                Contact = vendor.Contact,
                Address = vendor.Address
            };
        }

        [HttpPost]
        public async Task<ActionResult<VendorDto>> CreateVendor(VendorDto vendorDto)
        {
            var vendor = new Vendor
            {
                Vendor_Name = vendorDto.Vendor_Name,
                Contact = vendorDto.Contact,
                Address = vendorDto.Address
            };

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();

            vendorDto.Vendor_Id = vendor.Vendor_Id;
            return CreatedAtAction(nameof(GetVendor), new { id = vendor.Vendor_Id }, vendorDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(int id, VendorDto vendorDto)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();

            vendor.Vendor_Name = vendorDto.Vendor_Name;
            vendor.Contact = vendorDto.Contact;
            vendor.Address = vendorDto.Address;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVendor(int id)
        {
            var vendor = await _context.Vendors.FindAsync(id);
            if (vendor == null) return NotFound();

            _context.Vendors.Remove(vendor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
