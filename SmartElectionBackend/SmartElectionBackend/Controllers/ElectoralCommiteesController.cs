using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNamespace;
using SmartElectionBackend.Data;
using SmartElectionBackend.Models;

namespace SmartElectionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElectoralCommiteesController : ControllerBase
    {
        private readonly SmartElectionContext _context;
        private readonly Client client = new Client("https://localhost44340", new System.Net.Http.HttpClient());

        public ElectoralCommiteesController(SmartElectionContext context)
        {
            _context = context;
        }

        // GET: api/ElectoralCommitees
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ElectoralCommitee>>> GetElectoralCommitees()
        {
            return await _context.ElectoralCommitees.ToListAsync();
        }

        // GET: api/ElectoralCommitees/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ElectoralCommitee>> GetElectoralCommitee(int id)
        {
            var electoralCommitee = await _context.ElectoralCommitees.FindAsync(id);

            if (electoralCommitee == null)
            {
                return NotFound();
            }

            return electoralCommitee;
        }

        // PUT: api/ElectoralCommitees/5
        [HttpPut]
        public async Task<IActionResult> PutIoT(int id, string url, string model)
        {
            var commitee = await _context.ElectoralCommitees.FindAsync(id);
            commitee.IoTs.Add(new IoT()
            {
                Id = url,
                Model = model,
                ElectoralCommiteeId = id
            });
            await _context.SaveChangesAsync();
            return Accepted();
        }

        // POST: api/ElectoralCommitees
        [HttpPost]
        public async Task<ActionResult<ElectoralCommitee>> PostElectoralCommitee(string location, string info, string mail, string phone)
        {
            var electoralCommitee = new ElectoralCommitee()
            {
                Location = location,
                AdditionalInformation = info,
                Email = mail,
                Phone = phone
            };
            _context.ElectoralCommitees.Add(electoralCommitee);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetElectoralCommitee", new { id = electoralCommitee.Id }, electoralCommitee);
        }

        // DELETE: api/ElectoralCommitees/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ElectoralCommitee>> DeleteElectoralCommitee(int id)
        {
            var electoralCommitee = await _context.ElectoralCommitees.FindAsync(id);
            if (electoralCommitee == null)
            {
                return NotFound();
            }
            var certificate = await _context.UserCertificates.Where(x => x.ElectoralCommiteeId == id).FirstOrDefaultAsync();
            if (!(certificate is null))
            {
                await client.DeleteCertificateAsync(certificate.SubjectName, certificate.IssuerName);
                _context.UserCertificates.Remove(certificate);
            }
            _context.ElectoralCommitees.Remove(electoralCommitee);
            await _context.SaveChangesAsync();

            return electoralCommitee;
        }

        private bool ElectoralCommiteeExists(int id)
        {
            return _context.ElectoralCommitees.Any(e => e.Id == id);
        }
    }
}
