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
    [Route("api/Cinema")]
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
            return _context.Cinemas;
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

        // PUT: api/Cinema/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCinema([FromRoute] long id, [FromBody] Cinema cinema)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cinema.Id)
            {
                return BadRequest();
            }

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

            return NoContent();
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