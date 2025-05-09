using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LIN.Contacts.Services;

public class Jwt
{

    /// <summary>
    /// Llave del token
    /// </summary>
    private static string JwtKey { get; set; } = string.Empty;


    /// <summary>
    /// Inicia el servicio Jwt
    /// </summary>
    public static void Open(IConfigurationManager configuration)
    {
        JwtKey = configuration["LIN:Jwt"];
    }


    /// <summary>
    /// Genera un JSON Web Token
    /// </summary>
    /// <param name="user">Modelo de usuario</param>
    internal static string Generate(ProfileModel user)
    {

        // Configuración

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));

        // Credenciales
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        // Reclamaciones
        var claims = new[]
        {
            new Claim(ClaimTypes.PrimarySid, user.Id.ToString()), new Claim(ClaimTypes.UserData, user.AccountId.ToString())
        };

        // Expiración del token
        var expiración = DateTime.Now.AddHours(5);

        // Token
        var token = new JwtSecurityToken(null, null, claims, null, expiración, credentials);

        // Genera el token
        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    /// <summary>
    /// Valida un JSON Web token
    /// </summary>
    /// <param name="token">Token a validar</param>
    internal static JwtModel Validate(string token)
    {
        try
        {

            // Configurar la clave secreta
            var key = Encoding.ASCII.GetBytes(JwtKey);

            // Validar el token
            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = true
            };

            try
            {

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;


                // 
                _ = int.TryParse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.PrimarySid)?.Value, out var id);

                _ = int.TryParse(jwtToken.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.UserData)?.Value, out var accountId);


                // Devuelve una respuesta exitosa
                return new()
                {
                    IsAuthenticated = true,
                    AccountId = accountId,
                    ProfileId = id
                };

            }
            catch (SecurityTokenException)
            {

            }


        }
        catch { }

        return new();

    }

}