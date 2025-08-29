using Microsoft.AspNetCore.SignalR;

namespace Lead2Buy.API.Hubs
{
    // Hubs precisam herdar da classe Hub do SignalR
    public class NotificationHub : Hub
    {
        // Este método pode ser chamado por um cliente para entrar em um grupo, por exemplo.
        // Por enquanto, o deixaremos simples. A mágica acontecerá quando o back-end
        // enviar mensagens para os clientes conectados.
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }
    }
}