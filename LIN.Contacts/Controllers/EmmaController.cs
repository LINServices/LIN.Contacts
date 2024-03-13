using LIN.Contacts.Memory;
using LIN.Types.Emma.Models;
using System.Text;

namespace LIN.Contacts.Controllers;


[Route("Emma")]
public class EmmaController : ControllerBase
{



    /// <summary>
    /// Consulta para LIN Allo Emma.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    /// <param name="consult">Consulta.</param>
    [HttpPost]
    public async Task<HttpReadOneResponse<ResponseIAModel>> Assistant([FromHeader] string tokenAuth, [FromBody] string consult)
    {



        HttpClient client = new HttpClient();

        client.DefaultRequestHeaders.Add("token", tokenAuth);
        client.DefaultRequestHeaders.Add("useDefaultContext", true.ToString().ToLower());


        var request = new LIN.Types.Models.EmmaRequest
        {
            AppContext = "contacts",
            Asks = consult
        };



        StringContent stringContent = new(Newtonsoft.Json.JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var result = await client.PostAsync("http://api.emma.linapps.co/emma", stringContent);


        var ss = await result.Content.ReadAsStringAsync();


        dynamic fin = Newtonsoft.Json.JsonConvert.DeserializeObject(ss);


        // Respuesta
        return new ReadOneResponse<ResponseIAModel>()
        {
            Model = new()
            {
                IsSuccess = true,
                Content = fin?.result
            },
            Response = Responses.Success
        };

    }



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


        string final = $$""""

                        Estos son los contactos que tiene el usuario:

                        {{getProf?.StringOfContacts()}}

                        Estos son comandos:
                        
                        {
                          "name": "#say",
                          "description": "Utiliza esta función para decirle algo al usuario como saludos o responder a preguntas.",
                          "example":"#say('Hola')",
                          "parameters": {
                            "properties": {
                              "content": {
                                "type": "string",
                                "description": "contenido"
                              }
                            }
                          }
                        }

                        El formato para responder con comandos es:
                        "#NombreDelComando(Propiedades en orden separados por coma si es necesario)"
                        
                        
                        IMPORTANTE:
                        No en todos los casos en necesario usar comandos, solo úsalos cuando se cumpla la descripción.

                        NUNCA debes inventar comandos nuevos, solo puedes usar los que ya existen.

                        """";
        return new ReadOneResponse<object>()
        {
            Model = final,
            Response = Responses.Success
        };

    }


}