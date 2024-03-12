using LIN.Contacts.Memory;
using LIN.Types.Emma.Models;

namespace LIN.Contacts.Controllers;


[Route("Emma")]
public class EmmaController : ControllerBase
{



    /// <summary>
    /// Emma IA.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="consult">Prompt.</param>
    [HttpGet]
    public async Task<HttpReadOneResponse<object>> RequestFromEmma([FromHeader] string tokenAuth)
    {

        // Validar token.
        var response = await LIN.Access.Auth.Controllers.Authentication.Login(tokenAuth);


        if (response.Response != Responses.Success)
        {
            return new ReadOneResponse<object>()
            {
                Model = "Este usuario no autenticado en LIN Contactos."
            };
        }

        // 
        var profile = await Data.Profiles.ReadByAccount(response.Model.Id);


        if (profile.Response != Responses.Success)
        {
            return new ReadOneResponse<object>()
            {
                Model = "Este usuario no tiene una cuenta en LIN Contactos."
            };
        }


        var getProf = Mems.Sessions[profile.Model.Id];

        if (getProf == null)
        {

            getProf = new MemorySession()
            {
                Profile = profile.Model,
                Contactos = (await Data.Contacts.ReadAll(profile.Model.Id)).Models,
            };
            Mems.Sessions.Add(getProf);
        }


        var final = getProf?.StringOfContacts() ?? "No hay contactos";

        return new ReadOneResponse<object>()
        {
            Model = final,
            Response = Responses.Success
        };

    }


}