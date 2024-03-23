namespace LIN.Contacts.Services.Models;


public class JwtModel
{

    /// <summary>
    /// El token esta autenticado.
    /// </summary>
    public bool IsAuthenticated { get; set; }


    /// <summary>
    /// Id de la cuenta.
    /// </summary>
    public int AccountId { get; set; }


    /// <summary>
    /// Id del perfil.
    /// </summary>
    public int ProfileId { get; set; }
   

}