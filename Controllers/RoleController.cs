using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
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
    }
}