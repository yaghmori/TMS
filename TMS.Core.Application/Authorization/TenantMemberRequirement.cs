using Microsoft.AspNetCore.Authorization;

namespace TMS.Core.Application.Authorization
{
    public class TenantMemberRequirement : IAuthorizationRequirement
    {
        public TenantMemberRequirement()
        {

        }
    }

}
