using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LIN.Contacts.Services;


public class App
{


    /// <summary>
    /// Llave del token
    /// </summary>
    public static string AppCode { get; private set; } = string.Empty;



    /// <summary>
    /// Inicia el servicio Jwt
    /// </summary>
    public static void Open()
    {
        AppCode = Configuration.GetConfiguration("LIN:AppKey");
    }



}