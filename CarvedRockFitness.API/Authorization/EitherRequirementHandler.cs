using Microsoft.AspNetCore.Authorization;

namespace CarvedRockFitness.API.Authorization;

public class EitherRequirementHandler(IServiceProvider serviceProvider) : AuthorizationHandler<EitherRequirementRequirement>
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EitherRequirementRequirement requirement)
    {

        var authorizationService = _serviceProvider.GetRequiredService<IAuthorizationService>();

        var firstRequirement = await authorizationService.AuthorizeAsync(context.User, context.Resource, [requirement.FirstRequirement]);
        if (firstRequirement.Succeeded)
        {
            context.Succeed(requirement);
            return;
        }
        var secondRequirement = await authorizationService.AuthorizeAsync(context.User, context.Resource, [requirement.SecondRequirement]);
        if (secondRequirement.Succeeded)
        {
            context.Succeed(requirement);
            return;
        }

        context.Fail();
    }
}
