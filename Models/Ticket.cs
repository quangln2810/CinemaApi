using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaApi.Models
{
    public class Ticket
    {
        public Ticket() { }
        public Ticket(long idUser, long idSchedule, string seat)
        {
            IdUser = idUser;
            IdSchedule = idSchedule;
            Seat = seat;
        }
        [Key]
        public long Id { get; set; }
        [Required]
        public string Seat { get; set; }
        [Required]
        public DateTime BuyDate { get; set; }
        [Required]
        public long IdSchedule { get; set; }
        public Schedule Schedule { get; set; }
        [Required]
        public long IdUser { get; set; }
        public User User { get; set; }
    }

}
