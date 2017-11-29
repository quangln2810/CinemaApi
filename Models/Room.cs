using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CinemaApi.Models
{
    public partial class Room
    {
        public Room() { }
        public Room(string roomName)
        {
            Name = roomName;
        }
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Seats { get; set; }
        [Required]
        public long CinemaId { get; set; }
        public Cinema Cinema { get; set; }
    }
}
