using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IFanScheduleEvaluator, FanScheduleEvaluator>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.ExecuteSqlRaw("""
        CREATE TABLE IF NOT EXISTS "FanSchedules" (
            "devicename" TEXT NOT NULL CONSTRAINT "PK_FanSchedules" PRIMARY KEY,
            "stop_start_time" TEXT NOT NULL,
            "stop_end_time" TEXT NOT NULL,
            "wifi_name" TEXT NOT NULL,
            "wifi_pwd" TEXT NOT NULL,
            "run_seconds" INTEGER NOT NULL,
            "stop_seconds" INTEGER NOT NULL,            
            "is_enabled" INTEGER NOT NULL DEFAULT 1,
            CONSTRAINT "CK_FanSchedules_RunSeconds_Positive" CHECK (run_seconds > 0),
            CONSTRAINT "CK_FanSchedules_StopSeconds_NonNegative" CHECK (stop_seconds >= 0)
        );
        """);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
