using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PartController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PartDto>>> GetParts()
        {
            return await _context.Parts
                .Select(p => new PartDto
                {
                    Part_Id = p.Part_Id,
                    Part_Name = p.Part_Name,
                    Part_Price = p.Part_Price,
                    Stock = p.Stock
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PartDto>> GetPart(int id)
        {
            var part = await _context.Parts
                .FirstOrDefaultAsync(p => p.Part_Id == id);

            if (part == null) return NotFound();

            return new PartDto
            {
                Part_Id = part.Part_Id,
                Part_Name = part.Part_Name,
                Part_Price = part.Part_Price,
                Stock = part.Stock
            };
        }

        [HttpPost]
        public async Task<ActionResult<PartDto>> CreatePart(PartDto partDto)
        {
            var part = new Part
            {
                Part_Name = partDto.Part_Name,
                Part_Price = partDto.Part_Price,
                Stock = partDto.Stock
            };

            _context.Parts.Add(part);
            await _context.SaveChangesAsync();

            partDto.Part_Id = part.Part_Id;
            return CreatedAtAction(nameof(GetPart), new { id = part.Part_Id }, partDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePart(int id, PartDto partDto)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return NotFound();

            part.Part_Name = partDto.Part_Name;
            part.Part_Price = partDto.Part_Price;
            part.Stock = partDto.Stock;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePart(int id)
        {
            var part = await _context.Parts.FindAsync(id);
            if (part == null) return NotFound();

            _context.Parts.Remove(part);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
