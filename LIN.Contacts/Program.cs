global using Http.ResponsesList;
global using LIN.Contacts;
global using LIN.Contacts.Data;
global using LIN.Contacts.Services;
global using LIN.Modules;
global using LIN.Types.Auth.Abstracts;
global using LIN.Types.Contacts.Models;
global using LIN.Types.Responses;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
        });
});


var sqlConnection = builder.Configuration["ConnectionStrings:somee"] ?? string.Empty;

// Servicio de BD
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(sqlConnection);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



try
{
    // Si la base de datos no existe
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
    var res = dataContext.Database.EnsureCreated();
}
catch
{
}
app.UseCors("AllowAnyOrigin");


app.UseSwagger();
app.UseSwaggerUI(config =>
{
    config.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
    config.RoutePrefix = string.Empty;
});


app.UseHttpsRedirection();

app.UseAuthorization();

Conexión.SetStringConnection(sqlConnection);
Jwt.Open();
App.Open();

app.MapControllers();

app.Run();
