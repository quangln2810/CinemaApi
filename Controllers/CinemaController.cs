using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Cinema")]
    [Authorize(Roles="Administrator")]
    public class CinemaController : Controller
    {
        private readonly CinemaContext _context;

        public CinemaController(CinemaContext context)
        {
            _context = context;
        }

        // GET: api/Cinema
        [HttpGet]
        public IEnumerable<Cinema> GetCinema()
        {
            Console.WriteLine(_context.Cinemas.Include(cinema => cinema.Rooms).ToHashSet());
            return _context.Cinemas.Include(cinema => cinema.Rooms).ToHashSet();
        }

        // GET: api/Cinema/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCinema([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cinema = await _context.Cinemas.SingleOrDefaultAsync(m => m.Id == id);

            if (cinema == null)
            {
                return NotFound();
            }

            return Ok(cinema);
        }

        // PUT: api/Cinema
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCinema([FromBody] Cinema cinema)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(cinema).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CinemaExists(cinema.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(cinema);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateCinema([FromRoute] long id, [FromBody] Cinema editedCinema)
        {
            var cinema = await _context.Cinemas.FirstOrDefaultAsync(c => c.Id == id);
            Helpers.UpdatePartial(cinema, editedCinema);
            _context.Entry(cinema).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CinemaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(cinema);
        }
        
        // POST: api/Cinema
        [HttpPost]
        public async Task<IActionResult> PostCinema([FromBody] Cinema cinema)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Cinemas.Add(cinema);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCinema", new { id = cinema.Id }, cinema);
        }

        // DELETE: api/Cinema/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCinema([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cinema = await _context.Cinemas.SingleOrDefaultAsync(m => m.Id == id);
            if (cinema == null)
            {
                return NotFound();
            }

            _context.Cinemas.Remove(cinema);
            await _context.SaveChangesAsync();

            return Ok(cinema);
        }

        private bool CinemaExists(long id)
        {
            return _context.Cinemas.Any(e => e.Id == id);
        }
    }
}