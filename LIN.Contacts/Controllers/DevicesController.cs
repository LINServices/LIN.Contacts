namespace LIN.Contacts.Controllers;


[Route("device")]
public class DevicesController : ControllerBase
{


    /// <summary>
    /// Obtener dispositivos.
    /// </summary>
    /// <param name="token">Token de acceso.</param>
    [HttpGet]
    [LocalToken]
    public HttpReadAllResponse<DeviceModel> Devices([FromHeader] string token)
    {

        // Información del token.
        var tokenInfo = HttpContext.Items[token] as JwtModel ?? new();

        // Obtener los dispositivos.
        var devices = ContactsHub.List.Where(t => t.Key == tokenInfo.ProfileId).FirstOrDefault();

        // Respuesta.
        return new ReadAllResponse<DeviceModel>()
        {
            Response = Responses.Success,
            Models = devices.Value
        };
    }


}