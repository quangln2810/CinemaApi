using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Ticket")]
    [Authorize(Roles = "Administrator")]
    public class TicketController : Controller
    {
        private readonly CinemaContext _context;

        public TicketController(CinemaContext context)
        {
            _context = context;
        }

        // GET: api/Ticket
        [HttpGet]
        public IEnumerable<Ticket> GetTickets()
        {
            return _context.Tickets
                .Include(t => t.Schedule)
                    .ThenInclude(s => s.Movie)
                .Include(t => t.Schedule)
                    .ThenInclude(s => s.Room)
                .Include(t => t.User).ToList();
        }

        // GET: api/Ticket/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = await _context.Tickets
                .Include(t => t.Schedule)
                .Include(t => t.User)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }

        // PUT: api/Ticket/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTicket([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(ticket.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateTicket([FromRoute] long id, [FromBody] Ticket editedTicket)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(c => c.Id == id);
            Helpers.UpdatePartial(ticket, editedTicket);
            _context.Entry(ticket).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TicketExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(ticket);
        }

        // POST: api/Ticket
        [HttpPost]
        public async Task<IActionResult> PostTicket([FromBody] Ticket ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTicket", new { id = ticket.Id }, ticket);
        }

        // DELETE: api/Ticket/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicket([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = await _context.Tickets.SingleOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();

            return Ok(ticket);
        }

        private bool TicketExists(long id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        /// <summary>
        /// Should be called when payment success to create ticket
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="idSchedule"></param>
        /// <param name="seat"></param>
        /// <returns>return true if success, else return false</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        public bool CreateTicket(long idUser, long idSchedule, string seat)
        {
            if (_context.Tickets.Any(t => t.UserId == idUser && t.ScheduleId == idSchedule && t.Seat == seat))
            {
                return false;
            }
            Ticket ticket = new Ticket(idUser, idSchedule, seat);
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
            return true;
        }
    }
}