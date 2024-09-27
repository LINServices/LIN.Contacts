using LIN.Contacts.Services.Authentication;

namespace LIN.Contacts.Controllers;

[Route("[controller]")]
public class ProfileController(ICreateProfileService createService) : ControllerBase
{

    /// <summary>
    /// Inicia una sesión de usuario.
    /// </summary>
    /// <param name="user">Usuario único</param>
    /// <param name="password">Contraseña del usuario</param>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> Login([FromQuery] string user, [FromQuery] string password)
    {

        // Validar entradas.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            return new(Responses.InvalidParam)
            {
                Message = "Usuario / contraseña no pueden estar vacíos"
            };

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

                    // Crear perfil.
                    var response = await createService.Create(authResponse.Model);

                    if (response.Response != Responses.Success)
                    {
                        return new ReadOneResponse<AuthModel<ProfileModel>>
                        {
                            Response = Responses.UnavailableService,
                            Message = response.Message
                        };
                    }

                    profile = response;
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
                    // Crear perfil.
                    var createProfile = await createService.Create(response.Model);

                    if (createProfile.Response != Responses.Success)
                    {
                        return new ReadOneResponse<AuthModel<ProfileModel>>
                        {
                            Response = Responses.UnavailableService,
                            Message = createProfile.Message
                        };
                    }

                    profile = createProfile;
                    break;
                }

            default:
                return new ReadOneResponse<AuthModel<ProfileModel>>
                {
                    Response = Responses.UnavailableService,
                    Message = "Un error grave ocurrió"
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