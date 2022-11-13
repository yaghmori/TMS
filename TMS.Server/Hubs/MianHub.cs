using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TMS.Shared.Constants;

namespace TMS.Web.Server.Hubs
{
    [Authorize]
    public class MainHub : Hub
    {

        public async Task SendMessageAsync(string sender ,string message ,List<string> userIds)
        {
            if (userIds == null)
                await Clients.All.SendAsync(EndPoints.Hub.ReceiveMessage,sender,message);
            else
                await Clients.Users(userIds.Select(x => x.ToLowerInvariant())).SendAsync(EndPoints.Hub.ReceiveMessage,sender, message);
        }

        public async Task RegenerateTokensAsync()
        {
            await Clients.All.SendAsync(EndPoints.Hub.ReceiveRegenerateTokens);
        }

        public async Task UpdateAuthStateAsync(List<string> userIds)
        {
            if (userIds == null)
                await Clients.All.SendAsync(EndPoints.Hub.ReceiveUpdateAuthState);
            else
                await Clients.Users(userIds.Select(x => x.ToLowerInvariant())).SendAsync(EndPoints.Hub.ReceiveUpdateAuthState);
        }

        public async Task UpdateUserAsync(List<string> userIds)
        {
            if (userIds == null)
                await Clients.All.SendAsync(EndPoints.Hub.ReceiveUpdateUser);
            else
                await Clients.Users(userIds.Select(x => x.ToLowerInvariant())).SendAsync(EndPoints.Hub.ReceiveUpdateUser);
        }

        public async Task UpdateCultureAsync(List<string> userIds)
        {
            if (userIds == null)
                await Clients.All.SendAsync(EndPoints.Hub.ReceiveUpdateCulture);
            else
                await Clients.Users(userIds.Select(x => x.ToLowerInvariant())).SendAsync(EndPoints.Hub.ReceiveUpdateCulture);
        }

        public async Task TerminateSessionAsync(List<string> userIds)
        {
            if (userIds == null)
                await Clients.All.SendAsync(EndPoints.Hub.ReceiveTerminateSession);
            else
                await Clients.Users(userIds.Select(x => x.ToLowerInvariant())).SendAsync(EndPoints.Hub.ReceiveTerminateSession);
        }


    }
}


