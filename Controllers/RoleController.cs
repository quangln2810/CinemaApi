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
    [Route("api/Role")]
    public class RoleController : Controller
    {
        private readonly CinemaContext _context;

        public RoleController(CinemaContext context)
        {
            _context = context;
        }

        // GET: api/Role
        [HttpGet]
        public IEnumerable<Role> GetRole()
        {
            return _context.Roles;
        }

        // GET: api/Role/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _context.Roles.SingleOrDefaultAsync(m => m.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            return Ok(role);
        }

        // PUT: api/Role/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRole([FromRoute] long id, [FromBody] Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != role.Id)
            {
                return BadRequest();
            }

            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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

        // POST: api/Role
        [HttpPost]
        public async Task<IActionResult> PostRole([FromBody] Role role)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRole", new { id = role.Id }, role);
        }

        // DELETE: api/Role/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole([FromRoute] long id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _context.Roles.SingleOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(role);
        }

        private bool RoleExists(long id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}