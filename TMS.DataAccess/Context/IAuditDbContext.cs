using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace TMS.DataAccess.Context
{
    public interface IAuditDbContext
    {
        public string? UserId { get; set; }
        public string? IpAddress { get; set; }

    }
}
