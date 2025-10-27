using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        // Resiliency for transient Azure SQL hiccups (Azure-friendly)
        sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
        // Give the DB more time on first requests / cold starts
        sql.CommandTimeout(60);
    }));

builder.Services
    .AddDefaultIdentity<IdentityUser>(o => o.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<project_1Context>();

// ---------------------------------------------
// Controllers / JSON / Swagger
// ---------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();

// ---------------------------------------------
// DI for app services
// ---------------------------------------------
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMemberService, MemberService>();

var app = builder.Build();

// ---------------------------------------------
// Pipeline
// ---------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseRequestLogging();          // custom logging middleware
app.UseGlobalExceptionHandler();  // custom exception handler

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Liveness (app up)
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// DB health (guaranteed even if controller discovery fails)
app.MapGet("/healthz/db", async (project_1Context db) =>
{
    try
    {
        var can = await db.Database.CanConnectAsync();
        return can
            ? Results.Ok(new { status = "ok", db = "connected" })
            : Results.Json(new { status = "degraded", db = "unreachable" }, statusCode: 503);
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = "error", db = ex.Message }, statusCode: 500);
    }
});

// Map MVC controllers (includes your HealthDbController too)
app.MapControllers();

app.Run();
