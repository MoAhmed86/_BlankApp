
namespace API.Models
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string RawaafedUserName { get; set; }
        public string RawaafedPassword { get; set; }

        public int PageSize { get; set; }
    }
}
