using LIN.Contacts.Memory;

namespace LIN.Contacts.Controllers;


[Route("contacts")]
public class ContactsController : ControllerBase
{


    /// <summary>
    /// Crear contacto.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="model">Modelo.</param>
    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromBody] ContactModel model)
    {

        // Info del token.
        var (isValid, profileId, _) = Jwt.Validate(token);

        // Si el token es invalido.
        if (!isValid)
            return new CreateResponse()
            {
                Message = "Token invalido",
                Response = Responses.Unauthorized
            };

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
            Id = profileId
        };

        // Crear el contacto
        var response = await Data.Contacts.Create(model);

        return response;

    }



    /// <summary>
    /// Obtiene los contactos asociados a un perfil.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<ContactModel>> ReadAll([FromHeader] string token)
    {

        // Info dek token
        var (isValid, profileId, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new ReadAllResponse<ContactModel>()
            {
                Message = "Token invalido",
                Response = Responses.Unauthorized
            };

        // Obtiene los contactos
        var all = await Data.Contacts.ReadAll(profileId);

        // Registra en el memory
        var profileOnMemory = Mems.Sessions[profileId];

        if (profileOnMemory == null)
        {
            Mems.Sessions.Add(new()
            {
                Contactos = all.Models,
                Profile = new()
                {
                    Id = profileId
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
    public async Task<HttpReadOneResponse<ContactModel>> Read([FromQuery] int id, [FromHeader] string token)
    {

        // Info dek token
        var (isValid, profile, _) = Jwt.Validate(token);

        // Token es invalido.
        if (!isValid)
            return new ReadOneResponse<ContactModel>()
            {
                Message = "Token invalido",
                Response = Responses.Unauthorized
            };

        // Validar IAM.
        var authorization = await Data.Contacts.Iam(id, profile);

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
            Response = Responses.Success
        };

    }


}