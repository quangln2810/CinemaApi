using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace CinemaApi.Models
{
    public partial class User: IdentityUser<long>
    {
        [Key]
        public override long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public override string Email { get; set; }
        [EmailAddress]
        public override string UserName { get; set; }
        [Url]
        public string Avatar { get; set; }
        public string Address { get; set; }
    }
}
