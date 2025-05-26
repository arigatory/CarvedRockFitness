using CarvedRockFitness.API.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CarvedRockFitness.API.Authorization;

public class MustOwnRoutineHandler(
    CarvedRockFitnessDbContext carvedRockFitnessDbContext
    ) : AuthorizationHandler<MustOwnRoutineRequirement>

{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MustOwnRoutineRequirement requirement)
    {
        var httpContext = context.Resource as DefaultHttpContext;
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

        if (!int.TryParse(context.User.Claims
            .FirstOrDefault(c => c.Type == "sub")?.Value, out var userId))
        {
            context.Fail();
            return;
        }

        if (!await carvedRockFitnessDbContext.Routines.AnyAsync(r => r.Id == routineId && r.UserId == userId))
        {
            context.Fail();
            return;
        }

        context.Succeed(requirement);
    }
}
