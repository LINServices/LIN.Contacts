using LIN.Types.Cloud.Identity.Models;

namespace LIN.Contacts.Services.Authentication;

public class CreateProfileService : ICreateProfileService
{

    /// <summary>
    /// Crear perfil.
    /// </summary>
    /// <param name="account">Modelo de la cuenta.</param>
    public async Task<ReadOneResponse<ProfileModel>> Create(AccountModel account)
    {

        // Crear en la BD.
        var createResponse = await Profiles.Create(new()
        {
            Account = account,
            Profile = new()
            {
                AccountId = account.Id,
                Creation = DateTime.Now
            }
        });

        // Validar respuesta.
        if (createResponse.Response == Responses.Success)
            return new(Responses.Success, createResponse.Model);

        // Error.
        return new(Responses.UnavailableService)
        {
            Message = "Un error grave ocurrió"
        };
    }

}