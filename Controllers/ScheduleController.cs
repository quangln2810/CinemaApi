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
    [Route("api/Schedule")]
    [Authorize(Roles = "Administrator, Manager")]
    public class ScheduleController : Controller
    {
        private readonly CinemaContext _context;

        public ScheduleController(CinemaContext context)
        {
            _context = context;
        }

        // GET: api/Schedule
        [HttpGet]
        [AllowAnonymous]
        public IEnumerable<object> GetSchedules()
        {
            return _context.Schedules
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Select(s => new {
                s.Id,
                s.IdMovie,
                s.IdRoom,
                s.Movie,
                s.Room,
                s.Showtime
                });
        }

        // GET: api/Schedule/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSchedule([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var schedule = await _context.Schedules
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return Ok(schedule);
        }

        // PUT: api/Schedule/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSchedule([FromBody] Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(schedule.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(schedule);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] long id, [FromBody] Schedule editedSchedule)
        {
            var schedule = await _context.Schedules.FirstOrDefaultAsync(c => c.Id == id);
            Helpers.UpdatePartial(schedule, editedSchedule);
            _context.Entry(schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(schedule);
        }

        // POST: api/Schedule
        [HttpPost]
        public async Task<IActionResult> PostSchedule([FromBody] Schedule schedule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSchedule", new { id = schedule.Id }, schedule);
        }

        // DELETE: api/Schedule/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var schedule = await _context.Schedules.SingleOrDefaultAsync(m => m.Id == id);
            if (schedule == null)
            {
                return NotFound();
            }

            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();

            return Ok(schedule);
        }

        private bool ScheduleExists(long id)
        {
            return _context.Schedules.Any(e => e.Id == id);
        }
    }
}