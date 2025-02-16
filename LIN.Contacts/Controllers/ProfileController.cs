using LIN.Contacts.Services.Authentication;
using LIN.Types.Cloud.Identity.Models;

namespace LIN.Contacts.Controllers;

[Route("[controller]")]
[RateLimit(requestLimit: 5, timeWindowSeconds: 60, blockDurationSeconds: 120)]
public class ProfileController(ICreateProfileService createService) : ControllerBase
{

    /// <summary>
    /// Iniciar sesión.
    /// </summary>
    /// <param name="user">Usuario.</param>
    /// <param name="password">Contraseña.</param>
    [HttpGet("login")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> Login([FromQuery] string user, [FromQuery] string password)
    {

        // Validar parámetros.
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
            return new(Responses.InvalidParam)
            {
                Message = "Usuario / contraseña no pueden estar vacíos"
            };

        // Autenticar.
        var authResponse = await Access.Auth.Controllers.Authentication.Login(user, password);
        return await HandleAuthenticationResponse(authResponse);
    }


    /// <summary>
    /// Iniciar sesión con token.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet("login/token")]
    public async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> LoginToken([FromHeader] string token)
    {
        var authResponse = await Access.Auth.Controllers.Authentication.Login(token);
        return await HandleAuthenticationResponse(authResponse);
    }


    /// <summary>
    /// Manejar la respuesta de autenticación.
    /// </summary>
    /// <param name="authResponse">Respuesta.</param>
    private async Task<HttpReadOneResponse<AuthModel<ProfileModel>>> HandleAuthenticationResponse(ReadOneResponse<AccountModel> authResponse)
    {

        // Falla la autenticación.
        if (authResponse.Response != Responses.Success)
            return new ReadOneResponse<AuthModel<ProfileModel>>
            {
                Message = "Autenticación fallida",
                Response = authResponse.Response
            };

        // Leer la cuenta.
        var profile = await Profiles.ReadByAccount(authResponse.Model.Id);

        // Si no existe el perfil, crearlo.
        if (profile.Response == Responses.NotExistProfile)
        {
            var createProfileResponse = await createService.Create(authResponse.Model);
            if (createProfileResponse.Response != Responses.Success)
            {
                return new ReadOneResponse<AuthModel<ProfileModel>>
                {
                    Response = Responses.UnavailableService,
                    Message = createProfileResponse.Message
                };
            }
            profile = createProfileResponse;
        }

        // Error grave.
        if (profile.Response != Responses.Success)
        {
            return new ReadOneResponse<AuthModel<ProfileModel>>
            {
                Response = Responses.UnavailableService,
                Message = "Un error grave ocurrió"
            };
        }

        // Generar token.
        var token = Jwt.Generate(profile.Model);
        return new ReadOneResponse<AuthModel<ProfileModel>>
        {
            Response = Responses.Success,
            Message = "Success",
            Model = new AuthModel<ProfileModel>
            {
                Account = authResponse.Model,
                TokenCollection = new Dictionary<string, string>
                {
                    { "identity", authResponse.Token }
                },
                Profile = profile.Model
            },
            Token = token
        };
    }

}
