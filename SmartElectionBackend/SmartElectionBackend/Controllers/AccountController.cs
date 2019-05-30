using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartElectionBackend.Models;

namespace SmartElectionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpPost("/register")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostRegisterAsync([FromBody]RegisterViewModel model)
        {
            await RoleInitializer.InitializeAsync(_userManager, _roleManager);
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
                        return await PostLoginAsync(new LoginViewModel()
                        {
                            Email = model.Email,
                            Password = model.Password,
                            RememberMe = false
                        });
                    }
                    return NotFound();
                }
                return UnprocessableEntity();
            }
            return BadRequest();
        }

        [HttpPost("/login")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostLoginAsync([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (response.Succeeded)
                {
                    return Ok((await _userManager.FindByEmailAsync(model.Email)).Id);
                }
                return Unauthorized();
            }
            return BadRequest();
        }

        [HttpPost("/logout")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return Accepted();
        }
    }
}