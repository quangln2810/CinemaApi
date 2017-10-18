using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Models
{
    public enum Genres
    {
        Action, Adventure, Animation, Biography, Comedy, Crime, Documentary, Drama,
        Family, Fantasy, History, Horror, Music, Musical, Mystery, Romance, SciFi,
        Sport, Thriller, War
    }
    public class Movie
    {
        [Key]
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Poster { get; set; }
        [Required]
        public string Trailer { get; set; }
        [Required]
        public Genres Genre { get; set; }
        [Required]
        public TimeSpan Duration { get; set; }
    }
}
