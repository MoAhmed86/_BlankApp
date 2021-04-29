using System.Linq;
using Core.DTOs.Identity;
using Core.Resources;
using System.Collections.Generic;

namespace Core.DTOs.Search
{
    public class RoleSearchModel
    {
        public RoleSearchModel()
        {
            StatusList = new List<LookupItemDto>();
            StatusList.Add(new LookupItemDto()
            {
                Id = 0,
                DescAr = Resource.Active,
            });
            StatusList.Add(new LookupItemDto()
            {
                Id = 1,
                DescAr = Resource.InActive,
            });
        }

        public string RoleName { get; set; }
        public string ClaimType { get; set; }
        public bool? IsDeleted { get; set; }

        public int PageIndex { get; set; }

        public IList<LookupItemDto> StatusList { get; set; }
    }
}
