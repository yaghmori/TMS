using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.Core.Domain.Entities;

namespace TMS.Shared.Responses
{
    public class UserSettingsResponse
    {
        public string Id { get; set; }
        public string? DefaultTenantId { get; set; }
        public string Theme { get; set; } = string.Empty;
        public bool RightToLeft { get; set; } = false;
        public bool DarkMode { get; set; } = false;
        public string Culture { get; set; } = "en-US";
        public string? UserId { get; set; } = string.Empty;
        public string? UserEmail { get; set; }=string.Empty;

    }
}
