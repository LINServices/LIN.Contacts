using LIN.Access.OpenIA.ModelsData;
using LIN.Contacts.Memory;

namespace LIN.Contacts.Controllers;


[Route("Emma")]
public class EmmaController : ControllerBase
{





    [HttpPost]
    public async Task<HttpReadOneResponse<Message>> ReadAll([FromHeader] string token, [FromBody] string consult)
    {

        var (isValid, profileID, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new ReadOneResponse<Message>()
            {
                Response = Responses.Unauthorized
            };
        }

        var getProf = Mems.Sessions[profileID];

        var emma = new Access.OpenIA.IA(Configuration.GetConfiguration("openIa:key"));

        // Carga el modelo
        emma.LoadWho();
        emma.LoadRecomendations();
        emma.LoadPersonality();
        emma.LoadSomething($""" 
                           Estas en el contexto de LIN Contacts, la app de agenda de contactos de LIN Platform.
                           Estos son los contactos que tiene el usuario: {getProf?.StringOfContacts()}
                           Recuerda que el usuario puede preguntar información acerca de sus contactos
                           """);

        emma.LoadSomething($""" 
                           El usuario tiene {getProf?.Contactos.Count} contactos asociados a su cuenta.
                           """);

        var result = await emma.Respond(consult);

        return new ReadOneResponse<Message>()
        {
            Model = result,
            Response = Responses.Success
        };

    }




}