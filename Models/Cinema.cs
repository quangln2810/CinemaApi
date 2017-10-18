using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaApi.Models
{
    public partial class Cinema
    {
        public Cinema()
        {
            Rooms = new HashSet<Room>();
        }
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
