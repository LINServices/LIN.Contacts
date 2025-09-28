using LIN.Types.Cloud.Identity.Models;
using LIN.Types.Cloud.Identity.Models.Identities;

namespace LIN.Contacts.Services.Authentication;

public class CreateProfileService(Persistence.Data.Profiles profiles) : ICreateProfileService
{

    /// <summary>
    /// Crear perfil.
    /// </summary>
    /// <param name="account">Modelo de la cuenta.</param>
    public async Task<ReadOneResponse<ProfileModel>> Create(AccountModel account)
    {

        // Crear en la BD.
        var createResponse = await profiles.Create(new()
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