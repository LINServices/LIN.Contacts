

// Servicio de errores.
Logger.AppName = "LIN.Contacts";

// Constructor.
var builder = WebApplication.CreateBuilder(args);

// CORS.
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


// Obtiene el string de conexión SQL.
var sqlConnection = builder.Configuration["ConnectionStrings:somee"] ?? string.Empty;

// Servicio de BD
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(sqlConnection);
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



try
{
    // Si la base de datos no existe.
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
    var res = dataContext.Database.EnsureCreated();
}
catch (Exception ex)
{
    _ = Logger.Log(ex, 3);
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

LIN.Access.Auth.Build.Init();

app.MapControllers();

app.Run();