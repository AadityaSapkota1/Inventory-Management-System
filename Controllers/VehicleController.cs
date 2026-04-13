using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Models;
using VehicleManagementAPI.DTOs;

namespace VehicleManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VehicleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<VehicleDto>>> GetUserVehicles(int userId)
        {
            var vehicles = await _context.Vehicles
                .Where(v => v.User_Id == userId)
                .Select(v => new VehicleDto
                {
                    Vehicle_Id = v.Vehicle_Id,
                    Vehicle_Number = v.Vehicle_Number,
                    Vehicle_Type = v.Vehicle_Type,
                    User_Id = v.User_Id
                })
                .ToListAsync();

            return Ok(vehicles);
        }

        [HttpPost]
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
            return CreatedAtAction(nameof(GetUserVehicles), new { userId = vehicle.User_Id }, vehicleDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, VehicleDto vehicleDto)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            vehicle.Vehicle_Number = vehicleDto.Vehicle_Number;
            vehicle.Vehicle_Type = vehicleDto.Vehicle_Type;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
