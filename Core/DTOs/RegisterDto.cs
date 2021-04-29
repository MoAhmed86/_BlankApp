
namespace Core.DTOs
{
    public class RegisterDto
    {
        public int? Id { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsMale { get; set; }
        public bool? IsSpecialNeeds { get; set; }
    }
}