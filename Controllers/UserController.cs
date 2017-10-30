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
using System.IO;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace CinemaApi.Controllers
{
    public interface IMessageService
    {
        Task Send(string email, string subject, string message);
    }

    public class FileMessageService : IMessageService
    {
        Task IMessageService.Send(string email, string subject, string message)
        {
            var emailMessage = $"To: {email}\nSubject: {subject}\nMessage: {message}\n\n";

            File.AppendAllText("emails.txt", emailMessage);

            return Task.FromResult(0);
        }
    }

    [Produces("application/json")]
    [Route("api/User")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMessageService _messageService;
        private readonly IdentityDbContext _identityDbContext;

        public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IMessageService messageService, IdentityDbContext identityDbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _messageService = messageService;
            _identityDbContext = identityDbContext;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Register(string email, string password, string repassword)
        {
            if (!IsValidEmail(email))
            {
                ModelState.AddModelError(string.Empty, "Email is not valid");
            }
            if (password != repassword)
            {
                ModelState.AddModelError(string.Empty, "Password don't match");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new IdentityUser
            {
                UserName = email,
                Email = email
            };
            var userCreationResult = await _userManager.CreateAsync(user, password);
            if (!userCreationResult.Succeeded)
            {
                foreach (var error in userCreationResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(ModelState);
            }

            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var tokenVerificationUrl = Url.Action("VerifyEmail", "User", new { id = user.Id, token = emailConfirmationToken }, Request.Scheme);

            await _messageService.Send(email, "Verify your email", $"Click <a href=\"{tokenVerificationUrl}\">here</a> to verify your email");
            
            return NoContent();
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
        public async Task<IActionResult> Login(string email, string password, bool rememberMe)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User is not exist");
                return BadRequest(ModelState);
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Confirm your email first");
                return BadRequest(ModelState);
            }
            bool isLoginSucceed = await IsValidLogin(user, password, rememberMe);
            if (!isLoginSucceed)
            {
                ModelState.AddModelError(string.Empty, "Invalid login");
                return BadRequest(ModelState);
            }

            return new ObjectResult(GenerateToken(user));
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

        private async Task<bool> IsValidLogin(IdentityUser user, string password, bool rememberMe)
        {
            var passwordSignInResult = await _signInManager.PasswordSignInAsync(user, password, isPersistent: rememberMe, lockoutOnFailure: false);
            return passwordSignInResult.Succeeded;
        }

        private string GenerateToken(IdentityUser user)
        {
            Claim[] claims = new Claim[] {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("a secret key min 16 charaters"));
            var token = new JwtSecurityToken(new JwtHeader(new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)), new JwtPayload(claims));
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // GET: api/User
        [HttpGet]
        [Authorize]
        public IEnumerable<IdentityUser> GetUser()
        {
            return _identityDbContext.Users.ToList();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser([FromRoute] string email)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        //// PUT: api/User/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser([FromRoute] long id, [FromBody] User user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    user.Password = HashPassword(user.Password);
        //    _context.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// PATCH: api/User/5
        //[HttpPatch("{id}")]
        //public async Task<IActionResult> UpdateUser([FromRoute] long id, [FromBody] JsonPatchDocument<User> userPatch)
        //{
        //    var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
        //    userPatch.ApplyTo(user, ModelState);
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    _context.Update(user).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return Ok(user);
        //}

        //// POST: api/User
        //[HttpPost]
        //public async Task<IActionResult> PostUser([FromBody] User user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    _context.Users.Add(user);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetUser", new { id = user.Id }, user);
        //}

        //// DELETE: api/User/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteUser([FromRoute] long id)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var user = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Users.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return Ok(user);
        //}

        //[HttpPost]
        //[Route("[action]")]
        //public async Task<IActionResult> EmailExist([FromBody] string email)
        //{
        //    return Ok(await IsEmailExist(email));
        //}

        //private async Task<bool> IsEmailExist(string email)
        //{
        //    if (await _context.Users.AnyAsync(user => user.Email == email))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //private bool UserExists(long id)
        //{
        //    return _context.Users.Any(e => e.Id == id);
        //}

        //private string HashPassword(string password)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        //    }
        //}
    }
}