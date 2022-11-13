using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using TMS.Shared.Constants;

namespace TMS.Web.Server.Hubs
{
    public class HubUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(ApplicationClaimTypes.UserId)?.Value.ToLower()!;
        }
    }

}
