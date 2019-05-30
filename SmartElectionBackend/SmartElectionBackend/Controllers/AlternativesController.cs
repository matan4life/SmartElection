using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNamespace;
using SmartElectionBackend.Data;
using SmartElectionBackend.Models;

namespace SmartElectionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlternativesController : ControllerBase
    {
        private readonly SmartElectionContext _context;
        private readonly Client client = new Client("https://localhost:44340", new System.Net.Http.HttpClient());
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public AlternativesController(SmartElectionContext context, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: api/Alternatives
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Alternative>>> GetAlternatives(int electionId)
        {
            return await _context.Alternatives.Where(x=>x.ElectionId == electionId).ToListAsync();
        }

        // GET: api/Alternatives/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Alternative>> GetAlternative(int id)
        {
            var alternative = await _context.Alternatives.FindAsync(id);

            if (alternative == null)
            {
                return NotFound();
            }

            return alternative;
        }

        // PUT: api/Alternatives/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlternative(int id, string name, string info)
        {
            var alternative = await _context.Alternatives.FindAsync(id);
            alternative.Name = name;
            alternative.Info = info;
            await _context.SaveChangesAsync();
            return Accepted();
        }

        // POST: api/Alternatives
        [HttpPost]
        public async Task<ActionResult<Alternative>> PostAlternative(string name, string info, int electionId)
        {
            var alternative = new Alternative()
            {
                Name = name,
                Info = info,
                ElectionId = electionId
            };
            _context.Alternatives.Add(alternative);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAlternative", new { id = alternative.Id }, alternative);
        }

        // DELETE: api/Alternatives/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Alternative>> DeleteAlternative(int id)
        {
            var alternative = await _context.Alternatives.FindAsync(id);
            if (alternative == null)
            {
                return NotFound();
            }

            _context.Alternatives.Remove(alternative);
            await _context.SaveChangesAsync();

            return alternative;
        }

        private bool AlternativeExists(int id)
        {
            return _context.Alternatives.Any(e => e.Id == id);
        }


    }
}
