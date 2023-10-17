using LIN.Contacts.Data;
using LIN.Contacts.Services;

namespace LIN.Contacts.Controllers;


[Route("contacts")]
public class ContactsController : ControllerBase
{

    [HttpPost]
    public async Task<HttpCreateResponse> Create([FromHeader] string token, [FromBody] ContactModel model)
    {

        var (isValid, profileId, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new CreateResponse()
            {
                Message = "Token invalido",
                Response = Responses.Unauthorized
            };
        }

        model.Im = new()
        {
            Id = profileId
        };

        var response = await Data.Contacts.Create(model);

        return response;

    }

    [HttpGet("all")]
    public async Task<HttpReadAllResponse<ContactModel>> ReadAll([FromHeader] string token)
    {

        var (isValid, profileId, _) = Jwt.Validate(token);

        if (!isValid)
        {
            return new ReadAllResponse<ContactModel>()
            {
                Message = "Token invalido",
                Response = Responses.Unauthorized
            };
        }

        var all = await Data.Contacts.ReadAll(profileId);

        return new ReadAllResponse<ContactModel>()
        {
            Models = all.Models,
            Response = Responses.Success
        };

    }

}