using LIN.Contacts.Memory;
using LIN.Types.Emma.Models;

namespace LIN.Contacts.Controllers;


[Route("Emma")]
public class EmmaController : ControllerBase
{





    [HttpPost]
    public async Task<HttpReadOneResponse<ResponseIAModel>> ReadAll([FromHeader] string token, [FromBody] string consult)
    {

        var (isValid, profileID, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new ReadOneResponse<ResponseIAModel>()
            {
                Response = Responses.Unauthorized
            };
        }

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