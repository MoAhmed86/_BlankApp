using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.DTOs.Identity
{
    public class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() { }

        public ApplicationRole(string name)
        {
            Name = name;
        }

        public bool? IsDeleted { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public int CreatedBy { get; set; }
        public DateTime? LastUpdateDate { get; set; }
        public int? LastUpdatedBy { get; set; }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}