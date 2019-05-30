using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    public class FingerController : ControllerBase
    {
        private readonly SmartElectionContext _context;
        private readonly Client client = new Client("https://localhost:44340", new System.Net.Http.HttpClient());

        public FingerController(SmartElectionContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostAddFinger(string userId, int commiteeId)
        {
            var commitee = await _context.ElectoralCommitees.Include(x => x.IoTs).Where(x => x.Id == commiteeId).FirstOrDefaultAsync();
            string url = commitee.IoTs.FirstOrDefault().Id;
            WebRequest request = WebRequest.Create("http://" + url + "/addfinger");
            request.Method = "GET";
            var response = (HttpWebResponse)await request.GetResponseAsync();
            string result = await new StreamReader(response.GetResponseStream()).ReadToEndAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                commitee.IoTs.FirstOrDefault().UserFingers.Add(new Models.UserFingers()
                {
                    Field = int.Parse(result),
                    IoTId = commitee.IoTs.FirstOrDefault().Id,
                    UserId = userId
                });
                await _context.SaveChangesAsync();
                return Ok("Finger successfully added");
            }
            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                return Forbid();
            }
            return Conflict();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFinger(string userId)
        {
            var fingers = _context.UserFingers.Where(x => x.UserId == userId).ToList();
            if (fingers.Count == 0) return NotFound();
            foreach (var finger in fingers)
            {
                string url = finger.IoTId;
                var request = WebRequest.Create("http://" + url + "/deletefinger/" + finger.Field.ToString());
                request.Method = "GET";
                var response = (HttpWebResponse)await request.GetResponseAsync();
            }
            return Accepted();
        }

        [HttpGet]
        public async Task<bool> GetFingerVerification(string userId, string iot)
        {
            var finger = await _context.UserFingers.Where(x => x.IoTId == iot && x.UserId == userId).FirstOrDefaultAsync();
            var request = WebRequest.Create("http://" + iot + "/searchfinger/" + finger.Field.ToString());
            request.Method = "GET";
            var response = (HttpWebResponse)await request.GetResponseAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return bool.Parse(await new StreamReader(response.GetResponseStream()).ReadToEndAsync());
            }
            return false;
        }
    }
}