namespace Core.DTOs.Search
{
    public class UserSearchModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public bool? IsDeleted { get; set; }

        public int PageIndex { get; set; }
    }
}
