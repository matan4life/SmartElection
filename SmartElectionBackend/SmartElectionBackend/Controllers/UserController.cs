using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartElectionBackend.Data;
using SmartElectionBackend.Models;

namespace SmartElectionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly SmartElectionContext _context;

        public UserController(UserManager<User> userManager, SignInManager<User> signInManager, SmartElectionContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(_userManager.Users.ToList());
        }

        [HttpGet("/role")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> IsInRole(string userId, string role)
        {
            return Ok(await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(userId), role));
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }
            var response = _userManager.Users.Where(x => x.Id == userId).ToList();
            if (response.Count == 0)
            {
                return NotFound();
            }
            return Ok(response.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Country = model.Country
                };
                var response = await _userManager.CreateAsync(user, model.Password);
                if (response.Succeeded)
                {
                    var result = await _userManager.AddToRoleAsync(user, "User");
                    if (result.Succeeded)
                    {
                        return Ok(user);
                    }
                    return NotFound();
                }
                return UnprocessableEntity();
            }
            return BadRequest();
        }

        [HttpPost("/admin")]
        public async Task<IActionResult> PostAdmin([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    Country = model.Country
                };
                var response = await _userManager.CreateAsync(user, model.Password);
                if (response.Succeeded)
                {
                    var result = await _userManager.AddToRoleAsync(user, "Admin");
                    if (result.Succeeded)
                    {
                        return Ok(user);
                    }
                    return NotFound();
                }
                return UnprocessableEntity();
            }
            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> PutUser(string userId, EditViewModel model)
        {
            if (string.IsNullOrEmpty(userId) || userId != model.Id)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (!(user is null))
            {
                user.Email = model.Email;
                user.UserName = model.Email;
                user.Country = model.Country;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Accepted();
                }
                return Conflict();
            }
            return NotFound();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (!(user is null))
            {
                _context.UserCertificates.RemoveRange(_context.UserCertificates.Where(x => x.UserId == userId));
                _context.UserFingers.RemoveRange(_context.UserFingers.Where(x => x.UserId == userId));
                _context.Elections.RemoveRange(_context.Elections.Where(x => x.UserId == userId));
                _context.Turnouts.RemoveRange(_context.Turnouts.Where(x => x.UserId == userId));
                await _context.SaveChangesAsync();
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Accepted();
                }
                return Conflict();
            }
            return NotFound();
        }
    } 
}