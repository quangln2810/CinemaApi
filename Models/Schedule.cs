using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CinemaApi.Models
{
    public class Schedule
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public DateTime Showtime { get; set; }
        [Required]
        public long MovieId { get; set; }
        public Movie Movie { get; set; }
        [Required]
        public long RoomId { get; set; }
        public Room Room { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
