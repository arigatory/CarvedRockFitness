using CarvedRockFitness.API.Authorization;
using CarvedRockFitness.API.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarvedRockFitness.API.Controllers;

[ApiController]
[Route("routines")]
public class RoutinesController : ControllerBase
{
    private readonly CarvedRockFitnessDbContext _context;

    public RoutinesController(CarvedRockFitnessDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    [Authorize(Policy = PolicyMetadata.MustOwnRoutine)]
    public async Task<IActionResult> GetRoutineById(int id)
    {
        var routine = await _context.Routines
            .Include(r => r.User)
            .ThenInclude(u => u.Trainer)
            .Where(r => r.Id == id)
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Description,
                Trainee = new
                {
                    r.User.Id,
                    r.User.Name,
                    r.User.Email
                },
                Trainer = r.User.Trainer != null ? new
                {
                    r.User.Trainer.Id,
                    r.User.Trainer.Name,
                    r.User.Trainer.Email
                } : null
            })
            .FirstOrDefaultAsync();

        if (routine == null)
        {
            return NotFound();
        }

        return Ok(routine);
    }
}
