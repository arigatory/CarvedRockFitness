using CarvedRockFitness.API.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CarvedRockFitness.API.Authorization;

public class MustTrainRoutineOwnerHandler(CarvedRockFitnessDbContext carvedRockFitnessDbContext)
    : AuthorizationHandler<MustTrainRoutineOwnerRequirement>
{
    private readonly CarvedRockFitnessDbContext _carvedRockFitnessDbContext = carvedRockFitnessDbContext;

    protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, MustTrainRoutineOwnerRequirement requirement)
    {
        var httpContext = (context.Resource as DefaultHttpContext);
        if (httpContext == null)
        {
            context.Fail();
            return;
        }

        var routeValues = httpContext.GetRouteData().Values;
        if (!routeValues.TryGetValue("id", out var idValue))
        {
            context.Fail();
            return;
        }

        if (!int.TryParse(idValue as string, out var routineId))
        {
            context.Fail();
            return;
        }

        if (!int.TryParse(
            context.User.Claims
            .FirstOrDefault(c => c.Type == "sub")?.Value, out var userId))
        {
            context.Fail();
            return;
        }

        var routine = await _carvedRockFitnessDbContext.Routines
            .Include(r => r.User)
            .ThenInclude(u => u.Trainer)
            .FirstOrDefaultAsync(r => r.Id == routineId);

        if (routine == null || routine.User.TrainerId != userId
            || routine.User.Role != requirement.TraineeRoleName
            || routine.User.Trainer?.Role != requirement.TrainerRoleName)
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
