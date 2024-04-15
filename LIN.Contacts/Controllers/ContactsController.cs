using Azure;
using LIN.Contacts.Services.Models;
using LIN.Types.Responses;
using Microsoft.AspNetCore.SignalR;

namespace LIN.Contacts.Controllers;


[Route("contacts")]
public class ContactsController(IHubContext<ContactsHub> hubContext) : ControllerBase
{



    /// <summary>
    /// Hub de contexto.
    /// </summary>
    private readonly IHubContext<ContactsHub> _hubContext = hubContext;




    /// <summary>
    /// Crear contacto.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="model">Modelo.</param>
    [HttpPost]
    [LocalToken]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromBody] ContactModel model)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Validar
        if (model.Nombre.Trim().Length <= 0)
            return new CreateResponse()
            {
                Message = "Parámetros inválidos",
                Response = Responses.InvalidParam
            };

        // Agrega de quien es el contacto
        model.Im = new()
        {
            Id = tokenInfo.ProfileId
        };

        // Crear el contacto
        var response = await Data.Contacts.Create(model);


        // Si fue correcto.
        if (response.Response == Responses.Success)
        {
            // Realtime.
            string groupName = $"group.{tokenInfo.ProfileId}";
            string command = $"addContact({response.LastID})";
            await _hubContext.Clients.Group(groupName).SendAsync("#command", new CommandModel()
            {
                Command = command
            });
        }

        return response;

    }



    /// <summary>
    /// Obtiene los contactos asociados a un perfil.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("all")]
    [LocalToken]
    public async Task<HttpReadAllResponse<ContactModel>> ReadAll([FromHeader] string token)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtiene los contactos
        var all = await Data.Contacts.ReadAll(tokenInfo.ProfileId);

        // Registra en el memory
        var profileOnMemory = Mems.Sessions[tokenInfo.ProfileId];

        if (profileOnMemory == null)
        {
            Mems.Sessions.Add(new()
            {
                Contactos = all.Models,
                Profile = new()
                {
                    Id = tokenInfo.ProfileId
                }
            });
        }
        else
        {
            profileOnMemory.Contactos = all.Models;
        }


        // Respuesta.
        return new ReadAllResponse<ContactModel>()
        {
            Models = all.Models,
            Response = Responses.Success
        };

    }



    /// <summary>
    /// Obtener un contacto.
    /// </summary>
    /// <param name="id">Id del contacto.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet]
    [LocalToken]
    public async Task<HttpReadOneResponse<ContactModel>> Read([FromQuery] int id, [FromHeader] string token)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Validar IAM.
        var authorization = await Data.Contacts.Iam(id, tokenInfo.ProfileId);

        if (authorization.Response != Responses.Success)
            return new ReadOneResponse<ContactModel>()
            {
                Message = "No tienes permiso para acceder a este contacto.",
                Response = Responses.Unauthorized
            };

        // Obtiene los contactos
        var model = await Data.Contacts.Read(id);

        // Respuesta.
        return new ReadOneResponse<ContactModel>()
        {
            Model = model.Model,
            Response = model.Response
        };

    }



    /// <summary>
    /// Eliminar un contacto.
    /// </summary>
    /// <param name="id">Id del contacto.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpDelete]
    [LocalToken]
    public async Task<HttpResponseBase> Delete([FromQuery] int id, [FromHeader] string token)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Validar IAM.
        var authorization = await Data.Contacts.Iam(id, tokenInfo.ProfileId);

        if (authorization.Response != Responses.Success)
            return new ReadOneResponse<ContactModel>()
            {
                Message = "No tienes permiso para acceder a este contacto.",
                Response = Responses.Unauthorized
            };

        // Obtiene los contactos
        var res = await Data.Contacts.Delete(id);

        // Si fue correcto.
        if (res.Response == Responses.Success)
        {
            // Realtime.
            string groupName = $"group.{tokenInfo.ProfileId}";
            string command = $"removeContact({id})";
            await _hubContext.Clients.Group(groupName).SendAsync("#command", new CommandModel()
            {
                Command = command
            });
        }

        // Respuesta.
        return new()
        {
            Response = res.Response
        };

    }



    /// <summary>
    /// Actualizar el contacto.
    /// </summary>
    /// <param name="updateModel">Modelo</param>
    /// <param name="token">Token.</param>
    [HttpPatch]
    [LocalToken]
    public async Task<HttpResponseBase> Update([FromBody] ContactModel updateModel, [FromHeader] string token)
    {

        // Información del token.
        JwtModel tokenInfo = HttpContext.Items[token] as JwtModel ?? new();


        // Validar IAM.
        var authorization = await Data.Contacts.Iam(updateModel.Id, tokenInfo.ProfileId);

        if (authorization.Response != Responses.Success)
            return new ReadOneResponse<ContactModel>()
            {
                Message = "No tienes permiso para acceder a este contacto.",
                Response = Responses.Unauthorized
            };

        // Obtiene los contactos
        var res = await Data.Contacts.Update(updateModel);

        // Si fue correcto.
        if (res.Response == Responses.Success)
        {
            // Realtime.
            string groupName = $"group.{tokenInfo.ProfileId}";
            string command = $"update({updateModel.Id})";
            await _hubContext.Clients.Group(groupName).SendAsync("#command", new CommandModel()
            {
                Command = command
            });
        }

        // Respuesta.
        return new()
        {
            Response = res.Response
        };

    }



}