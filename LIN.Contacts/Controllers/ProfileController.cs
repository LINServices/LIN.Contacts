namespace LIN.Contacts.Controllers;


[Route("[controller]")]
public class ProfileController : ControllerBase
{


    /// <summary>
    /// Inicia una sesión de usuario.
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="password">Contraseña del usuario</param>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> Login([FromQuery] string user, [FromQuery] string password)
    {

        // Comprobación
        if (!user.Any() || !password.Any())
            return new(Responses.InvalidParam);

        // Respuesta de autenticación.
        var authResponse = await Access.Auth.Controllers.Authentication.Login(user, password, App.AppCode);

        // Autenticación errónea.
        if (authResponse.Response != Responses.Success)
            return new ReadOneResponse<AuthModel<ProfileModel>>
            {
                Message = "Autenticación fallida",
                Response = authResponse.Response
            };


        // Obtiene el perfil
        var profile = await Profiles.ReadByAccount(authResponse.Model.Id);

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
                            AccountId = authResponse.Model.Id,
                            Creation = DateTime.Now
                        }
                    });

                    if (res.Response != Responses.Success)
                    {
                        return new ReadOneResponse<AuthModel<ProfileModel>>
                        {
                            Response = Responses.UnavailableService,
                            Message = "Un error grave ocurri�"
                        };
                    }

                    profile = res;
                    break;
                }

            default:
                return new ReadOneResponse<AuthModel<ProfileModel>>
                {
                    Response = Responses.UnavailableService,
                    Message = "Un error grave ocurri�"
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
                TokenCollection = new()
                {
                    {
                        "identity", authResponse.Token
                    }
                },
                Profile = profile.Model
            },
            Token = token
        };

    }



    /// <summary>
    /// Iniciar sesión con el token.
    /// </summary>
    /// <param name="token">Token</param>
    [HttpGet("login/token")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> LoginToken([FromQuery] string token)
    {

        // Login en LIN Server
        var response = await Access.Auth.Controllers.Authentication.Login(token);

        if (response.Response != Responses.Success)
            return new(response.Response);




        var profile = await Profiles.ReadByAccount(response.Model.Id);


        switch (profile.Response)
        {
            case Responses.Success:
                break;

            case Responses.NotExistProfile:
                {
                    var res = await Profiles.Create(new()
                    {
                        Account = response.Model ,
                        Profile = new()
                        {
                            AccountId = response.Model.Id,
                            Creation = DateTime.Now
                        }
                    });

                    if (res.Response != Responses.Success)
                    {
                        return new ReadOneResponse<AuthModel<ProfileModel>>
                        {
                            Response = Responses.UnavailableService,
                            Message = "Un error grave ocurri�"
                        };
                    }

                    profile = res;
                    break;
                }

            default:
                return new ReadOneResponse<AuthModel<ProfileModel>>
                {
                    Response = Responses.UnavailableService,
                    Message = "Un error grave ocurri�"
                };
        }





        var httpResponse = new ReadOneResponse<AuthModel<ProfileModel>>()
        {
            Response = Responses.Success,
            Message = "Success"

        };

        if (profile.Response == Responses.Success)
        {
            // Genera el token
            var tokenAcceso = Jwt.Generate(profile.Model);

            httpResponse.Token = tokenAcceso;
            httpResponse.Model.Profile = profile.Model;
        }

        httpResponse.Model.Account = response.Model;
        httpResponse.Model.TokenCollection = new()
        {
            {
                "identity", response.Token
            }
        };


        return httpResponse;

    }


}