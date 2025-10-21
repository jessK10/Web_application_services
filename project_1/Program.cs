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
// Pipeline
// ---------------------------------------------
app.UseSwagger();
app.UseSwaggerUI();

// ?? Custom middlewares (order matters: logging first, exception handler early)
app.UseRequestLogging();
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();