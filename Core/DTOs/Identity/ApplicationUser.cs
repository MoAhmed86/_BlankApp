using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Core.DTOs.Identity
{
    public class ApplicationUser : IdentityUser<int>
    {
        [MaxLength(100)]
        public string FullName { get; set; }
        [MaxLength(10)]
        public string IdentityNumber { get; set; }
        [MaxLength(50)]
        public string City { get; set; }
        [MaxLength(150)]
        public string Address { get; set; }

        public bool IsMale { get; set; }

        public bool? IsSpecialNeeds { get; set; }

        public bool? IsDeleted { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdatedBy { get; set; }

        public virtual ICollection<ApplicationUserRole> ApplicationUserRoles { get; set; }
    }
}
