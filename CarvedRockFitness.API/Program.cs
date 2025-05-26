using CarvedRockFitness.API.Authorization;
using CarvedRockFitness.API.DbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<CarvedRockFitnessDbContext>(
    dbContextOptions => dbContextOptions.UseSqlServer(
        builder.Configuration["ConnectionStrings:CarvedRockFitnessDB"]));

builder.Services.AddScoped<IAuthorizationHandler, MustOwnRoutineHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MustTrainRoutineOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, EitherRequirementHandler>();

builder.Services.AddAuthorization(authorizationOptions =>
{
    authorizationOptions.AddPolicy(PolicyMetadata.MustOwnRoutine, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.AddRequirements(new MustOwnRoutineRequirement());

    });

    authorizationOptions.AddPolicy(PolicyMetadata.MustOwnRoutineOrTrainOwner, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.AddRequirements(new EitherRequirementRequirement(
            new MustOwnRoutineRequirement(),
            new MustTrainRoutineOwnerRequirement("Trainer", "Trainee")));
    });
});

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options => options.MapInboundClaims = false);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
