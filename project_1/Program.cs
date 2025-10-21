using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using project_1.Data;
using project_1.Services;
using project_1.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------
// Database / Identity
// ---------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("project_1ContextConnection")
    ?? throw new InvalidOperationException("Connection string 'project_1ContextConnection' not found.");

builder.Services.AddDbContext<project_1Context>(opt =>
    opt.UseSqlServer(connectionString, sql =>
    {
        // Resiliency for transient Azure SQL hiccups (good for Azure)
        sql.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);

        // Give the DB more time on first requests / cold starts
        sql.CommandTimeout(60);
    }));

builder.Services.AddDefaultIdentity<IdentityUser>(o => o.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<project_1Context>();

// ---------------------------------------------
// Controllers and Swagger
// ---------------------------------------------
builder.Services.AddControllers()
    // Prevent cycles in case entities ever get serialized directly
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Optional: standardized RFC-7807 responses for errors
builder.Services.AddProblemDetails();

// ---------------------------------------------
// Dependency Injection for your app services
// ---------------------------------------------
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMemberService, MemberService>();

var app = builder.Build();

// ---------------------------------------------
// OPTIONAL (enable once on Azure to create tables, then remove):
// ---------------------------------------------
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<project_1Context>();
//     db.Database.Migrate(); // applies pending migrations on the configured DB
// }

// ---------------------------------------------
// Pipeline
// ---------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();

// Custom middlewares (order matters: logging first, exception handler early)
app.UseRequestLogging();
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ---------------------------------------------
// Health endpoints
// ---------------------------------------------

// Simple liveness (app started)
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// DB health (use this for Azure Health Check)
app.MapGet("/orm/health", async ([FromServices] project_1Context db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        if (!canConnect)
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);

        return Results.Ok(new { status = "ok", db = "connected" });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "DB connectivity failed",
            detail: ex.Message,
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
});

app.Run();
