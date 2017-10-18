using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApi.Models;

namespace CinemaApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Room")]
    public class RoomController : Controller
    {
        private readonly CinemaContext _context;

        public RoomController(CinemaContext context)
        {
            _context = context;
        }

        // GET: api/Room
        [HttpGet]
        public IEnumerable<Room> GetRoom()
        {
            return _context.Rooms;
        }

        // GET: api/Room/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = await _context.Rooms.SingleOrDefaultAsync(m => m.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        // PUT: api/Room/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom([FromRoute] long id, [FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != room.Id)
            {
                return BadRequest();
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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

        // POST: api/Room
        [HttpPost]
        public async Task<IActionResult> PostRoom([FromBody] Room room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = room.Id }, room);
        }

        // DELETE: api/Room/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var room = await _context.Rooms.SingleOrDefaultAsync(m => m.Id == id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        private bool RoomExists(long id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
}