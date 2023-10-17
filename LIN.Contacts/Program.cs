global using Microsoft.AspNetCore.Mvc;
global using Http.ResponsesList;
global using Microsoft.EntityFrameworkCore;
global using LIN.Types.Contacts.Models;
global using LIN.Contacts.Data;
global using LIN.Contacts;
global using LIN.Types.Responses;
global using LIN.Modules;
global using LIN.Types.Auth.Abstracts;
using LIN.Contacts.Services;

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


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

Conexi�n.SetStringConnection(sqlConnection);
Jwt.Open();
App.Open();

app.MapControllers();

app.Run();
