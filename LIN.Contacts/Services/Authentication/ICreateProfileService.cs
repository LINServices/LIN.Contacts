using LIN.Types.Cloud.Identity.Models;
using LIN.Types.Cloud.Identity.Models.Identities;

namespace LIN.Contacts.Services.Authentication;

public interface ICreateProfileService
{
    Task<ReadOneResponse<ProfileModel>> Create(AccountModel account);
}