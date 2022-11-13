namespace TMS.Shared.Responses
{
    public class UserResponse
    {

        public string Id { get; set; }
        public string? ProfileName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;
        public string? Image { get; set; } = default!;
        public string? Description { get; set; } = default!;
        public bool IsActive { get; set; } = true;
        public UserSettingsResponse Settings { get; set; } = new();

        public virtual ICollection<RoleResponse> Roles { get; set; } = new List<RoleResponse>();
        public virtual ICollection<TenantResponse> Tenants { get; set; } = new List<TenantResponse>();
        public virtual ICollection<UserClaimResponse> Claims { get; set; } = new List<UserClaimResponse>();
        public virtual ICollection<UserSessionResponse> ActiveSessions { get; set; } = new List<UserSessionResponse>();
        public string FullName => $"{FirstName} {LastName}";
        public int RolesCount => Roles.Count;
        public int TenantsCount => Tenants.Count;
        public int UserClaimsCount => Claims.Count;
        public int ActiveSessionsCount => ActiveSessions.Count;

    }
}
