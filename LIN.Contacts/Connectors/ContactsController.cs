namespace LIN.Contacts.Connectors;

[Route("[controller]")]
public class AssistantController(Persistence.Data.Profiles profiles, Persistence.Data.Contacts contacts) : ControllerBase
{

    /// <summary>
    /// Obtiene los contactos asociados a un perfil.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("all")]
    public async Task<HttpReadAllResponse<ContactModel>> ReadAll([FromHeader] string token)
    {

        var information = await LIN.Access.Auth.Controllers.Authentication.Login(token);

        if (information.Response != Responses.Success)
        {
            return new ReadAllResponse<ContactModel>()
            {
                Response = information.Response,
                Message = "Autenticación invalida."
            };
        }

        // Leer la cuenta.
        var profile = await profiles.ReadByAccount(information.Model.Id);

        if (profile.Response != Responses.Success)
        {
            return new ReadAllResponse<ContactModel>()
            {
                Response = profile.Response,
                Message = "No se encontró el perfil."
            };
        }

        // Obtiene los contactos
        var all = await contacts.ReadAll(profile.Model.Id);

        // Respuesta.
        return new ReadAllResponse<ContactModel>()
        {
            Models = all.Models,
            Response = Responses.Success
        };
    }

}