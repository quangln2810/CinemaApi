using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CinemaApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Role")]
    [Authorize(Roles="Administrator")]
    public class RoleController : Controller
    {
        private readonly CinemaContext _context;
        private readonly RoleManager<Role> _roleManager;

        public RoleController(CinemaContext context, RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: api/Role
        [HttpGet]
        public IEnumerable<Role> GetRole()
        {
            return _roleManager.Roles;
        }

        //// GET: api/Role/5
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetRole([FromRoute] string roleName)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (!await _roleManager.RoleExistsAsync(roleName))
        //    {
        //        return NotFound();
        //    }
        //    var role = await _roleManager.FindByNameAsync(roleName);
        //    return Ok(role);
        //}
        
        //// POST: api/Role
        //[HttpPost]
        //public async Task<IActionResult> AddRole([FromBody] Role role)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    await _roleManager.CreateAsync(role);
            
        //    return CreatedAtAction("GetRole", new { roleName = role.Name }, role);
        //}

        //// DELETE: api/Role/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteRole([FromRoute] string roleName)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    if (!await _roleManager.RoleExistsAsync(roleName))
        //    {
        //        return NotFound();
        //    }
        //    var role = await _roleManager.FindByNameAsync(roleName);
        //    var result = await _roleManager.DeleteAsync(role);
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(result.Errors);
        //    }
        //    return Ok(role);
        //}
    }
}