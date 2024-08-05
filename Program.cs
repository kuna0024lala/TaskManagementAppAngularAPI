using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using TaskManagementAppAngular.Data;
using TaskManagementAppAngular.Repositories.Implementation;
using TaskManagementAppAngular.Repositories.Interface;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<TaskDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("TaskDbConnectionString")));

builder.Services.AddScoped<ITaskRepository, TaskRepository>();

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(options =>
{
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.WithOrigins("http://localhost:4200");
});

app.UseAuthorization();

app.MapControllers();

app.Run();
