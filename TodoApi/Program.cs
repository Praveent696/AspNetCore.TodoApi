using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi.Configurations;
using TodoApi.Data;
using TodoApi.Extensions;
using TodoApi.Models;
using TodoApi.Models.Dto;
using TodoApi.Services.Abstrations;
using TodoApi.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(name: JwtBearerDefaults.AuthenticationScheme, securityScheme: new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference= new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id=JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
    });
});

builder.AddAppAuthetication();
builder.Services.AddAuthentication();

var app = builder.Build();
await SeedData(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// use jwt validation middleware

app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Seeds initial data into the application for testing purposes. It adds an admin user and a default user
/// into the system with predefined roles and credentials. This method is called during the application startup.
/// The `SeedData` method is responsible for seeding the database with essential initial data during the application startup. It ensures that there are at least two users in the system:
///   1. * *Admin User * *: With the role of "Admin".
///   2. **Default User**: With no special roles.
/// This method is executed during the application’s initialization and sets up the system with the required users and roles for access control.
/// </summary>
/// <param name="app">The instance of the WebApplication being configured.</param>
/// <returns>A task representing the asynchronous operation.</returns>
async Task SeedData(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var userService = services.GetRequiredService<IUserService>();

        // Add Admin User
        RegistrationRequestDto user = new()
        {
            FirstName = "Admin",
            LastName = "User",
            Gender = "MALE",
            Email = "admin@gmail.com",
            Password = "Admin@123",
            PhoneNumber = "1234567890",
            Age = 21
        };
        await userService.RegisterUser(user);
        await userService.AssignRole(user.Email, "Admin");

        // Add Default User
        user = new()
        {
            FirstName = "Default",
            LastName = "User",
            Gender = "MALE",
            Email = "default@gmail.com",
            Password = "Default@123",
            PhoneNumber = "1234567890",
            Age = 21
        };
        await userService.RegisterUser(user);
    }
}
