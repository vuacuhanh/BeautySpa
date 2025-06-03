using Microsoft.AspNetCore.SignalR;


namespace BeautySpa.Repositories.SignalR
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("id")?.Value;
        }
    }
}
