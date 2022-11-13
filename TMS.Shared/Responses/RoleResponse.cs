namespace TMS.Shared.Responses
{
    public class RoleResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<UserResponse> Users { get; set; } = new();
        public List<ClaimResponse> Claims { get; set; } = new();

        public int UsersCount => Users.Count;
        public string NormalizedName => Name.Normalize().ToUpper();
    }
}
