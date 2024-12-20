using DataAccessLayer.Data;
using DataAccessLayer.Interfaces;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using HMS.Api.App.Options;
using LMSApi.Seeders;
using MusicApp.App.Filters;
using Microsoft.Extensions.FileProviders;
using BusinessLayer.Interfaces;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using DataLayer.Interfaces;
using DataLayer.Repositories;
using BussinessLayer.Services;
using BussinessLayer.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSwaggerGen();

// Add CORS policy to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
        builder.AllowAnyOrigin() // Allow all origins
               .AllowAnyHeader() // Allow all headers
               .AllowAnyMethod()); // Allow all HTTP methods
});

// Register repositories and services for Customer
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();  // Customer Repository
builder.Services.AddScoped<ICustomerService, CustomerService>();        // Customer Service

builder.Services.AddScoped<IOperationRepository, OperationRepository>();  // Operation Repository
builder.Services.AddScoped<IOperationService, OperationService>();

builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<PermissionSeeder>();

builder.Services.AddHttpContextAccessor();
// Add IHttpClientFactory service
builder.Services.AddHttpClient();

// Add the BackgroundService
builder.Services.AddHostedService<KeepAliveService>();


// Configure JWT options
JwtOptions? jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
if (jwtOptions == null)
{
    throw new Exception("Jwt options not found");
}
builder.Services.AddSingleton(jwtOptions);

// Configure authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
    };
});

// Set up caching and session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
    options.Cookie.IsEssential = true; // Mark the session cookie as essential
});

// Add permission check filter
builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionCheckFilter>();
});

var app = builder.Build();

//Seed permissions data
await using (var scope = app.Services.CreateAsyncScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<PermissionSeeder>();
    await seeder.Seed();
}

//Enable Swagger in Development/Production
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dispatch API V1");
    c.RoutePrefix = String.Empty;
});
}

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();

//}

// Enable HTTPS redirection
app.UseHttpsRedirection();

app.UseRouting();

// Apply the "AllowAll" CORS policy
app.UseCors("AllowAll");

// Use Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();

// Enable Session support
app.UseSession();

// Register Customer API routes
app.MapControllers(); // This will map all controller routes, including Customer routes

app.Run();
