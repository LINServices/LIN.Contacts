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
    [HttpPost]
    public async Task<HttpReadOneResponse<ResponseIAModel>> ReadAll([FromHeader] string token, [FromBody] string consult)
    {

        // Información del token.
        var (isValid, profileID, _) = Jwt.Validate(token);

        // El token es invalido.
        if (!isValid)
            return new()
            {
                Message = "El token es invalido.",
                Response = Responses.Unauthorized
            };


        var getProf = Mems.Sessions[profileID];

        var iaModel = new Access.OpenIA.IAModelBuilder(Configuration.GetConfiguration("openIa:key"));

        iaModel.Load(IaConsts.Base);
        iaModel.Load(IaConsts.Personalidad);

        iaModel.Load($"""
                      Estas en el contexto de LIN Contacts, la app de agenda de contactos de LIN Platform.
                      Estos son los contactos que tiene el usuario: {getProf?.StringOfContacts()}
                      Recuerda que el usuario puede preguntar información acerca de sus contactos y deveras contestar acertadamente.
                      """);
        iaModel.Load($"""
                      El usuario tiene {getProf?.Contactos.Count} contactos asociados a su cuenta.
                      """);

        var final = await iaModel.Reply(consult);

        return new ReadOneResponse<ResponseIAModel>()
        {
            Model = final,
            Response = Responses.Success
        };

    }


}