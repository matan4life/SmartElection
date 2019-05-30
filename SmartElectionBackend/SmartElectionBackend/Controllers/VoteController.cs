using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyNamespace;
using SmartElectionBackend.Data;

namespace SmartElectionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        private readonly SmartElectionContext _context;
        private readonly Client client = new Client("https://localhost:44340", new System.Net.Http.HttpClient());

        public VoteController(SmartElectionContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(202)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PostVote(string userId, int electionId, int commiteeId, int alternativeId)
        {
            var election = await _context.Elections.FindAsync(electionId);
            if (election is null) return NotFound("No such election");
            var commitee = await _context.ElectoralCommitees.Include(x => x.IoTs).Where(x => x.Id == commiteeId).FirstOrDefaultAsync();
            if (commitee is null) return NotFound("No such a commitee");
            var agreement = _context.CommiteeAgreements.Where(x => x.ElectoralCommiteeId == commiteeId && x.ElectionId == electionId).FirstOrDefault();
            if (agreement is null) return Forbid("No agreement with election administration");
            var commiteeCertificate = _context.UserCertificates.Where(x => x.ElectoralCommiteeId == commiteeId).FirstOrDefault();
            var response = await client.GetSignatureVerificationAsync(commiteeCertificate.SubjectName, commiteeCertificate.IssuerName,
                agreement.CommiteeSign, Convert.ToBase64String(Encoding.Default.GetBytes(commiteeId.ToString())));
            if (!response) return Unauthorized("Fake signature");
            if (election.NeedsFingerprint)
            {
                if (_context.UserFingers.Where(x => x.UserId == userId).ToList().Count == 0) return Unauthorized("No finger in database");
                var iot = commitee.IoTs.FirstOrDefault();
                var verificationResult = await new FingerController(_context).GetFingerVerification(userId, iot.Id);
                if (!verificationResult) return Forbid("No finger match");
            }
            if (election.NeedsCertificate)
            {
                var certificateResult = await client.GetCertificateVerificationAsync(userId, election.UserId);
                if (!certificateResult) return Forbid("No valid certificate");
            }
            var alternative = await _context.Alternatives.FindAsync(alternativeId);
            alternative.Ballots.Add(new Models.Ballot()
            {
                AlternativeId = alternativeId,
                Date = DateTime.Now,
                ElectoralCommiteeId = commiteeId,
                CommiteeSignature = await client.GetSignedDataAsync(commiteeCertificate.SubjectName, commiteeCertificate.IssuerName,
                Convert.ToBase64String(Encoding.Default.GetBytes(commiteeId.ToString())))
            });
            _context.Turnouts.Add(new Models.Turnout()
            {
                Date = DateTime.Now,
                ElectionId = electionId,
                ElectoralCommiteeId = commiteeId,
                UserId = userId,
                ElectoralCommiteeSignature = await client.GetSignedDataAsync(commiteeCertificate.SubjectName, commiteeCertificate.IssuerName,
                Convert.ToBase64String(Encoding.Default.GetBytes(commiteeId.ToString()))),
                UserSignature = await client.GetSignedDataAsync(userId, election.UserId,
                Convert.ToBase64String(Encoding.Default.GetBytes(commiteeId.ToString())))
            });
            await _context.SaveChangesAsync();
            return Accepted();
        }

    }
}