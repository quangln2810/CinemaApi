using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaApi.Models
{
    public partial class Role: IdentityRole<long>
    {
        public Role(string roleName)
        {
            Name = roleName;
        }
        [Key]
        public override long Id { get; set; }
        [Required]
        public override string Name { get; set; }
    }
}
