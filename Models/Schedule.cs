using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Models
{
    public class Schedule
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public DateTime Showtime { get; set; }
        [Required]
        public long IdMovie { get; set; }
        public Movie Movie { get; set; }
        [Required]
        public long IdRoom { get; set; }
        public Room Room { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
