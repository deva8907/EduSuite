using EduSuite.ApiService.Features.Tenants.Services;
using EduSuite.Database;
using EduSuite.Database.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Add controllers and API explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register tenant and user providers before database
builder.Services.AddScoped<ITenantProvider, DesignTimeTenantProvider>();
builder.Services.AddScoped<ICurrentUserProvider, DesignTimeUserProvider>();

// Add EduSuite Database
builder.Services.AddEduSuiteDatabase(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Server=(localdb)\\mssqllocaldb;Database=EduSuite;Trusted_Connection=True;MultipleActiveResultSets=true");

// Add EduSuite services
builder.Services.AddScoped<ITenantService, TenantService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable routing and endpoint middleware
app.UseRouting();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.MapDefaultEndpoints();

app.Run();

public partial class Program { }
