using EMS.API.Controllers;
using EMS.API.Middleware;
using EMS.Core.DTOs.Documents;
using EMS.Core.Helpers;
using EMS.Data;
using EMS.Data.Models;
using EMS.Data.Repositories;
using EMS.Data.UnitOfWork;
using EMS.Service.Authentication;
using EMS.Service.Careers;
using EMS.Service.Departments;
using EMS.Service.Designations;
using EMS.Service.Email;
using EMS.Service.Employees;
using EMS.Service.FileStorage;
using EMS.Service.JobPostings;
using EMS.Service.Leave;
using EMS.Service.Roles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================
// 1. SERILOG CONFIGURATION
// ============================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)   // ← ADD THIS LINE — appsettings.json padikkum
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)   // explicit safety override
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();
// ============================
// 2. DATABASE (EF Core + MySQL)
// ============================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<EmsDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ============================
// 3. JWT AUTHENTICATION
// ============================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

// ============================
// 4. DEPENDENCY INJECTION (Repositories + Services)
// ============================
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, EMS.Data.UnitOfWork.UnitOfWork>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService,EmailService>();
builder.Services.AddSingleton<JwtTokenGenerator>();
builder.Services.AddScoped<IDesignationService, DesignationService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
//careers
builder.Services.AddScoped<ICareerService, CareerService>();
builder.Services.AddScoped<IJobPostingService, JobPostingService>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<FileSettings>(builder.Configuration.GetSection("FileSettings"));
builder.Services.AddScoped<IUrlHelperService, UrlHelperService>();

builder.Services.AddHttpContextAccessor();   // add this, if not already there
// already covered via open-generic registration


// ============================
// 5. CONTROLLERS + SWAGGER
// ============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Employee Management System API", Version = "v1" });

    // Enable JWT auth in Swagger UI
    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ============================
// 6. CORS (allow frontend to call this API)
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// ============================
// MIDDLEWARE PIPELINE (ORDER MATTERS)
// ============================

app.UseGlobalExceptionHandler();   // must be FIRST — catches everything below it

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAll");

app.UseAuthentication();           // must come BEFORE Authorization
app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting EMS API...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application failed to start");
}
finally
{
    Log.CloseAndFlush();
}