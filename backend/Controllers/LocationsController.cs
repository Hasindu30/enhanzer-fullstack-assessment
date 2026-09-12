using backend.Data;
using backend.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public LocationsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetLocations()
    {
        var locations = await _db.LocationDetails
            .OrderBy(l => l.LocationName)
            .Select(l => new LocationDto
            {
                LocationCode = l.LocationCode,
                LocationName = l.LocationName
            })
            .ToListAsync();

        return Ok(locations);
    }
}
