using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyNamespace;

namespace SmartElectionBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly Client client = new Client("https://localhost:44340", new System.Net.Http.HttpClient());

        [HttpGet]
        public async Task<IActionResult> GetCACertificate(string issuerName)
        {
            try
            {
                var response = await client.GetCACertificateAsync(issuerName);
                return Ok(response);
            }
            catch (SwaggerException<MyNamespace.ProblemDetails> e)
            {
                return StatusCode(e.StatusCode);
            }
        }
    }
}