namespace LIN.Contacts.Controllers;


[Route("contacts")]
public class ContactsController : ControllerBase
{


    /// <summary>
    /// Crear contacto
    /// </summary>
    /// <param name="token">Token de acceso</param>
    /// <param name="model">Modelo del contacto</param>
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
    /// Obtiene los contactos asociados a un perfil
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

        // Respuesta.
        return new ReadAllResponse<ContactModel>()
        {
            Models = all.Models,
            Response = Responses.Success
        };

    }


}