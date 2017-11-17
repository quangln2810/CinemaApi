using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CinemaApi.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.JsonPatch;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using static CinemaApi.Services.MailService;
using Microsoft.Extensions.Options;

namespace CinemaApi.Controllers
{
    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMessageService _messageService;
        private readonly JWTSettings _jwtSettings;
        private readonly CinemaContext _context;

        public UserController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            SignInManager<User> signInManager,
            IMessageService messageService,
            IOptions<JWTSettings> options,
            CinemaContext cinemaContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _messageService = messageService;
            _jwtSettings = options.Value;
            _context = cinemaContext;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(string password, [FromBody] User user)
        {
            if (!IsValidEmail(user.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is not valid");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            user.UserName = user.Email;
            var userCreationResult = await _userManager.CreateAsync(user, password);
            if (!userCreationResult.Succeeded)
            {
                foreach (var error in userCreationResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(ModelState);
            }
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenVerificationUrl = Url.Action("VerifyEmail", "User", new { id = user.Id, token = emailConfirmationToken }, Request.Scheme);

            await _messageService.Send(user.Email, "Verify your email", $"Click <a href=\"{tokenVerificationUrl}\">here</a> to verify your email");

            return Ok(tokenVerificationUrl);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> VerifyEmail(string id, string token)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return BadRequest("User is not valid");
            var emailConfirmationResult = await _userManager.ConfirmEmailAsync(user, token);
            if (!emailConfirmationResult.Succeeded) return BadRequest("Can not verify");
            return NoContent();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User is not exist");
                return BadRequest(ModelState);
            }
            //if (!user.EmailConfirmed)
            //{
            //    ModelState.AddModelError(string.Empty, "Confirm your email first");
            //    return BadRequest(ModelState);
            //}
            bool isLoginSucceed = await IsValidLogin(user, password);
            if (!isLoginSucceed)
            {
                ModelState.AddModelError(string.Empty, "Invalid login");
                return BadRequest(ModelState);
            }
            var token = await GenerateTokenAsync(user);
            return new ObjectResult(new { user.Id, user.Email, user.Avatar, user.Address, token });
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return BadRequest("User is not exist");
            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passwordResetUrl = Url.Action("ResetPassword", "User", new { id = user.Id, token = passwordResetToken }, Request.Scheme);

            await _messageService.Send(email, "Password reset", $"Click <a href=\"" + passwordResetUrl + "\">here</a> to reset your password");

            return NoContent();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> ResetPassword(string id, string token, string password, string repassword)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return BadRequest("User is not exist");
            if (password != repassword)
            {
                ModelState.AddModelError(string.Empty, "Passwords do not match");
                return BadRequest(ModelState);
            }

            var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, password);
            if (!resetPasswordResult.Succeeded)
            {
                foreach (var error in resetPasswordResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(ModelState);
            }

            return NoContent();
        }

        private bool IsValidEmail(string strIn)
        {
            if (String.IsNullOrEmpty(strIn))
                return false;
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private async Task<bool> IsValidLogin(User user, string password)
        {
            var passwordSignInResult = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            return passwordSignInResult.Succeeded;
        }

        private async Task<string> GenerateTokenAsync(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Iss, _jwtSettings.Issuer),
                new Claim(JwtRegisteredClaimNames.Aud, _jwtSettings.Audience),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds()}")
            };
            var roles = await _userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                var claim = new Claim(ClaimTypes.Role, role);
                claims.Add(claim);
            }
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var token = new JwtSecurityToken(new JwtHeader(new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)), new JwtPayload(claims));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [Route("List")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetUsers([FromQuery] string roleName = null)
        {
            IEnumerable<User> users;
            if (String.IsNullOrEmpty(roleName))
            {
                users = await _context.Users.ToListAsync();
            }
            else
            {
                users = await _userManager.GetUsersInRoleAsync(roleName);
            }
            return Ok(users.Select(async u =>
            {
                var role = await _userManager.GetRolesAsync(u);
                return new { u.Id, u.Email, u.Avatar, u.Address, Role = role };
            }));
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            var userEmail = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized();
            }
            var role = await _userManager.GetRolesAsync(user);
            return Ok(new { user.Id, user.Email, user.Avatar, user.Address, role = role });
        }

        [HttpPost]
        [Route("[action]")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> SetRole(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("User is not exist");
            }
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                return BadRequest("Role is not exist");
            }
            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return NoContent();
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] User editedUser)
        {
            var userEmail = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized();
            }
            Helpers.UpdatePartial(user, editedUser);
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(user);
        }

    }
}