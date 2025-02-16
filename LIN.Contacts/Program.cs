using Http.Extensions;
using LIN.Access.Auth;
using LIN.Contacts.Services.Authentication;

// Crear constructor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Obtiene el string de conexión SQL.
var sqlConnection = builder.Configuration["ConnectionStrings:somee"] ?? string.Empty;

// Servicio de BD
builder.Services.AddDbContext<Context>(options =>
{
    options.UseSqlServer(sqlConnection);
});

// Controladores.
builder.Services.AddLINHttp();

builder.Services.AddSignalR();
builder.Services.AddAuthenticationService(app: builder.Configuration["LIN:app"]);

// Services.
builder.Services.AddScoped<ContactsHubActions, ContactsHubActions>();
builder.Services.AddSingleton<ICreateProfileService, CreateProfileService>();

// Crear app.
var app = builder.Build();

try
{
    // Si la base de datos no existe.
    using var scope = app.Services.CreateScope();
    var dataContext = scope.ServiceProvider.GetRequiredService<Context>();
    var res = dataContext.Database.EnsureCreated();
}
catch (Exception)
{
}

app.UseLINHttp();

// Establecer string de conexión.
Conexión.SetStringConnection(sqlConnection);
Jwt.Open();
App.Open();

app.MapHub<ContactsHub>("/realTime/contacts");

app.MapControllers();

app.Run();