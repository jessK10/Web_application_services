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

// Liveness (app up) — your DB check is in HealthDbController at /healthz/db
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// Map MVC controllers (includes HealthDbController -> /healthz/db)
app.MapControllers();

app.Run();
