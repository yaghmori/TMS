using Microsoft.AspNetCore.Authorization;

namespace TMS.Core.Application.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }
        public string Permission { get; set; }
    }

}
