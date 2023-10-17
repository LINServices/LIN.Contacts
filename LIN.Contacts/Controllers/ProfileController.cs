using LIN.Contacts.Data;
using LIN.Contacts.Services;

namespace LIN.Contacts.Controllers;


[Route("profile")]
public class ProfileController : ControllerBase
{


    /// <summary>
    /// Inicia una sesión de usuario
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="password">Contraseña del usuario</param>
    /// <param name="app">Key de la app que solicita la información</param>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> Login([FromQuery] string user, [FromQuery] string password)
    {

        // Comprobación
        if (!user.Any() || !password.Any())
            return new(Responses.InvalidParam);

        // Respuesta de autenticación
        var authResponse = await Access.Auth.Controllers.Authentication.Login(user, password, App.AppCode);

        // Autenticación errónea
        if (authResponse.Response != Responses.Success)
            return new ReadOneResponse<AuthModel<ProfileModel>>
            {
                Message = "Autenticación fallida",
                Response = authResponse.Response
            };


        // Obtiene el perfil
        var profile = await Profiles.ReadByAccount(authResponse.Model.ID);

        switch (profile.Response)
        {
            case Responses.Success:
                break;
            case Responses.NotExistProfile:
                {
                    var res = await Profiles.Create(new()
                    {
                        Account = authResponse.Model,
                        Profile = new()
                        {
                            AccountId = authResponse.Model.ID,
                            Creation = DateTime.Now
                        }
                    });

                    if (res.Response != Responses.Success)
                    {
                        return new ReadOneResponse<AuthModel<ProfileModel>>
                        {
                            Response = Responses.UnavailableService,
                            Message = "Un error grave ocurrió"
                        };
                    }

                    profile = res;
                    break;
                }
            default:
                return new ReadOneResponse<AuthModel<ProfileModel>>
                {
                    Response = Responses.UnavailableService,
                    Message = "Un error grave ocurrió"
                };
        }


        // Genera el token
        var token = Jwt.Generate(profile.Model);

        return new ReadOneResponse<AuthModel<ProfileModel>>
        {
            Response = Responses.Success,
            Message = "Success",
            Model = new()
            {
                Account = authResponse.Model,
                LINAuthToken = authResponse.Token,
                Profile = profile.Model
            },
            Token = token
        };

    }



    /// <summary>
    /// Iniciar sesión
    /// </summary>
    /// <param name="token">Token</param>
    [HttpGet("login/token")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> LoginToken([FromQuery] string token)
    {

        // Login en LIN Server
        var response = await Access.Auth.Controllers.Authentication.Login(token);

        if (response.Response != Responses.Success)
            return new(response.Response);

        if (response.Model.Estado != Types.Auth.Enumerations.AccountStatus.Normal)
            return new(Responses.NotExistAccount);



        var profile = await Profiles.ReadByAccount(response.Model.ID);


        var httpResponse = new ReadOneResponse<AuthModel<ProfileModel>>()
        {
            Response = Responses.Success,
            Message = "Success",

        };

        if (profile.Response == Responses.Success)
        {
            // Genera el token
            var tokenAcceso = Jwt.Generate(profile.Model);

            httpResponse.Token = tokenAcceso;
            httpResponse.Model.Profile = profile.Model;
        }

        httpResponse.Model.Account = response.Model;
        httpResponse.Model.LINAuthToken = response.Token;


        return httpResponse;

    }



    /// <summary>
    /// Obtiene los miembros de una conversación
    /// </summary>
    /// <param name="id">ID de la conversación.</param>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("search")]
    public async Task<HttpReadAllResponse<SessionModel<ProfileModel>>> Search([FromQuery] string pattern, [FromHeader] string token)
    {

        // Busca el acceso
        var accounts = await Access.Auth.Controllers.Account.Search(pattern, token, false);

        // Si no tiene acceso
        if (accounts.Response != Responses.Success)
            return new ReadAllResponse<SessionModel<ProfileModel>>
            {
                Response = Responses.Unauthorized,
                Message = "No tienes acceso a LIN Identity"
            };


        var mappedIds = accounts.Models.Select(T => T.ID).ToList();

        var profiles = await Profiles.ReadByAccounts(mappedIds);


        var final = from P in profiles.Models
                    join A in accounts.Models
                    on P.AccountId equals A.ID
                    select new SessionModel<ProfileModel>
                    {
                        Account = A,
                        Profile = P
                    };

        // Retorna el resultado
        return new ReadAllResponse<SessionModel<ProfileModel>>
        {
            Response = Responses.Success,
            Models = final.ToList()
        };

    }


}