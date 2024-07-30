using LIN.Types.Cloud.Identity.Models;

namespace LIN.Contacts.Services.Authentication;

public interface ICreateProfileService
{
    Task<ReadOneResponse<ProfileModel>> Create(AccountModel account);
}