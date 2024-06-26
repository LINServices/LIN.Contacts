using Http.Services;

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