using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class ElectionsController : ControllerBase
    {
        private readonly SmartElectionContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly Client client = new Client("https://localhost:44340", new System.Net.Http.HttpClient());
        public ElectionsController(SmartElectionContext context, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: api/Elections
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Election>>> GetElections()
        {
            return await _context.Elections.ToListAsync();
        }

        // GET: api/Elections/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Election>> GetElection(int id)
        {
            var election = await _context.Elections.FindAsync(id);

            if (election == null)
            {
                return NotFound();
            }

            return election;
        }

        // PUT: api/Elections/5
        [HttpPut]
        public async Task<IActionResult> PutElectionAgreement(int electionId, int commiteeId)
        {
            var commitee = await _context.ElectoralCommitees.FindAsync(commiteeId);
            var election = await _context.Elections.FindAsync(electionId);
            if (election.NeedsFingerprint)
            {
                var iots = await _context.IoTs.Where(x => x.ElectoralCommiteeId == commiteeId).ToListAsync();
                if (iots.Count == 0) return Forbid();
            }
            var commiteeCert = await client.PostCreateCertificateAsync(commiteeId.ToString(), election.UserId, election.Start, election.End);
            commitee.UserCertificates.Add(new UserCertificates()
            {
                ElectoralCommiteeId = commitee.Id,
                Id = commiteeCert.Thumbprint,
                IssuerName = commiteeCert.Issuer.Substring(3),
                SubjectName = commiteeCert.Subject.Substring(3)
            });
            election.CommiteeAgreements.Add(new CommiteeAgreement()
            {
                ElectionId = electionId,
                ElectoralCommiteeId = commiteeId,
                CommiteeSign = await client.GetSignedDataAsync(commiteeCert.Subject.Substring(3), commiteeCert.Issuer.Substring(3), Convert.ToBase64String(
                    Encoding.Default.GetBytes(commiteeId.ToString())))
            });
            await _context.SaveChangesAsync();
            return Accepted();
        }

        [HttpPut("/addCertificate")]
        public async Task<IActionResult> PutUserCertificate(int electionId, string userId)
        {
            var election = await _context.Elections.FindAsync(electionId);
            var user = await _userManager.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user is null)
            {
                return NotFound();
            }
            var certificate = await client.PostCreateCertificateAsync(userId, election.UserId, election.Start, election.End);
            user.UserCertificates.Add(new UserCertificates()
            {
                IssuerName = certificate.Issuer.Substring(3),
                SubjectName = certificate.Subject.Substring(3),
                UserId = user.Id,
                User = user
            });
            await _context.SaveChangesAsync();
            return Accepted();
        }

        // POST: api/Elections
        [HttpPost]
        public async Task<ActionResult<Election>> PostElection(DateTimeOffset start, DateTimeOffset end, bool needsCertificate, bool needsFingerprint, string calculationType, string userId)
        {
            var election = new Election()
            {
                Start = start,
                End = end,
                NeedsCertificate = needsCertificate,
                NeedsFingerprint = needsFingerprint,
                CalculationType = calculationType
            };
            var user = await _userManager.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user is null)
            {
                return NotFound();
            }
            election.User = user;
            election.UserId = user.Id;
            _context.Elections.Add(election);
            if (election.NeedsCertificate)
            {
                var response = await client.PostCreateCACertificateAsync(user.Id, election.Start, election.End);
                _context.UserCertificates.Add(new UserCertificates()
                {
                    ElectionId = election.Id,
                    Id = response.Thumbprint,
                    SubjectName = response.Subject.Substring(3),
                    IssuerName = response.Issuer.Substring(3)
                });
            }
            await _context.SaveChangesAsync();
            return Ok(election);
        }

        // DELETE: api/Elections/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Election>> DeleteElection(int id)
        {
            var election = await _context.Elections.FindAsync(id);
            if (election == null)
            {
                return NotFound();
            }
            var root = await _context.UserCertificates.Where(x=>x.ElectionId == id).FirstOrDefaultAsync();
            var certificates = await _context.UserCertificates.Where(x => x.IssuerName == root.IssuerName && x.SubjectName!=root.IssuerName).ToListAsync();
            await client.DeleteCACertificateAsync(root.IssuerName);
            foreach (var cert in certificates)
            {
                await client.DeleteCertificateAsync(cert.SubjectName, cert.IssuerName);
            }

            _context.Elections.Remove(election);
            _context.UserCertificates.Remove(root);
            _context.UserCertificates.RemoveRange(certificates);
            await _context.SaveChangesAsync();

            return election;
        }

        private bool ElectionExists(int id)
        {
            return _context.Elections.Any(e => e.Id == id);
        }

        [HttpGet("/winner")]
        [ProducesResponseType(typeof(Alternative), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWinner(int electionId)
        {
            var election = await _context.Elections.Include(x => x.Alternatives).ThenInclude(x => x.Ballots).Where(x => x.Id == electionId).FirstOrDefaultAsync();
            var count = election.Alternatives.SelectMany(x => x.Ballots).Count();
            if (count == 0) return BadRequest("No ballots yet!");
            var ballots = election.Alternatives.SelectMany(x => x.Ballots).GroupBy(x => x.AlternativeId);
            var result = ballots.FirstOrDefault();
            foreach (var group in ballots)
            {
                if (group.Count() > result.Count()) result = group;
            }
            return Ok(await _context.Alternatives.FindAsync(result.Key));
        }
    }
}
