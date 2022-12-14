namespace TMS.Shared.Responses
{
    public class UserSessionResponse
    {
        public string Id { get; set; } = default!;
        public string UserId { get; set; }
        public virtual string LoginProvider { get; set; } = default!;
        public virtual string BuildVersion { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = default!;
        public DateTime? RefreshTokenExpires { get; set; }
        public DateTime? StartDate { get; set; }
        public string? SessionIpAddress { get; set; }


    }

}
