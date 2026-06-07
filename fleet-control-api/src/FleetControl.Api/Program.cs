using FleetControl.CrossCutting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=fleet-control.db";

builder.Services.AddFleetControlServices(connectionString);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Fleet Control API", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("FleetControlPolicy", policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? ["http://localhost:5173", "http://localhost:3000"])
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.Services.ApplyMigrations();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fleet Control API v1");
    c.RoutePrefix = string.Empty;
});

app.UseSerilogRequestLogging();
app.UseCors("FleetControlPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
