
namespace Core.DTOs
{
    public class LookupItemDto
    {
        public int Id { get; set; }
        public string DescAr { get; set; }
        public string DescEn { get; set; }
        public int ParentId { get; set; }
    }

    public class OrganizerDto : LookupItemDto
    {
        public string PlatformName { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
