using Http.Extensions;
using LIN.Access.Auth;
using LIN.Contacts.Persistence.Extensions;
using LIN.Contacts.Services.Authentication;

// Crear constructor.
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Controladores.
builder.Services.AddLINHttp();

builder.Services.AddSignalR();
builder.Services.AddAuthenticationService(app: builder.Configuration["LIN:app"]);

// Services.
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddScoped<ContactsHubActions, ContactsHubActions>();
builder.Services.AddScoped<ICreateProfileService, CreateProfileService>();

// Crear app.
var app = builder.Build();

app.UseLINHttp();
app.UsePersistence();

// Establecer string de conexión.
Jwt.Open();

app.MapHub<ContactsHub>("/realTime/contacts");

app.MapControllers();

app.Run();