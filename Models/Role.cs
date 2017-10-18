using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaApi.Models
{
    public partial class Role
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
