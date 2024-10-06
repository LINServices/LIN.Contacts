using Microsoft.AspNetCore.SignalR;

namespace LIN.Contacts.Hubs;

public sealed class ContactsHubActions(IHubContext<ContactsHub> hubContext)
{

    /// <summary>
    /// Enviar comando.
    /// </summary>
    /// <param name="profile">Id del perfil.</param>
    /// <param name="conversation">Id de la conversación.</param>
    public async Task SendCommand(int profile, string command)
    {
        // Realtime.
        string groupName = $"group.{profile}";
        await hubContext.Clients.Group(groupName).SendAsync("#command", new CommandModel()
        {
            Command = command
        });
    }

}