using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartElectionCertificationCentre.Data;

namespace SmartElectionCertificationCentre.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController : ControllerBase
    {
        private readonly PrivateContext context;

        public CertificateController(PrivateContext _context)
        {
            context = _context;
        }

        /// <summary>
        /// Gets the root certificate with concrete name
        /// </summary>
        /// <param name="issuerName">The election commitee name</param>
        /// <returns>The root certificate</returns>
        /// <response code="200">Returns the root certificate with specified issuer name</response>
        /// <response code="400">The issuer name string was empty</response>
        /// <response code="404">Certificate with specified issuer name was not found in the store</response>
        [HttpGet("/certificate/issuerName")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(X509Certificate2), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCACertificate(string issuerName)
        {
            if (string.IsNullOrEmpty(issuerName))
            {
                return BadRequest("Empty input!");
            }
            var response = Certificate.GetCACertificate(issuerName);
            if (!(response is null))
            {
                return Ok(response);
            }
            return NotFound();
        }

        /// <summary>
        /// Creates a root certificate for election process
        /// </summary>
        /// <param name="issuerName">The election commitee name</param>
        /// <param name="start">Start date</param>
        /// <param name="end">Expiration date</param>
        /// <returns>A root certificate from the store</returns>
        /// <response code="201">Returns newly created root certificate</response>
        /// <response code="400">The election commitee name was empty</response>
        /// <response code="409">Server created certificate, but can't get it from the store</response>
        /// <response code="422">The certificate with the same issuer name exists</response>
        [HttpPost("/certificate/root")]
        [ProducesResponseType(typeof(X509Certificate2), 201)]
        [Produces("application/json")]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PostCreateCACertificate(string issuerName, DateTimeOffset start, DateTimeOffset end)
        {
            if (string.IsNullOrEmpty(issuerName))
            {
                return BadRequest("Empty input");
            }
            if (!(Certificate.GetCACertificate(issuerName) is null))
            {
                return UnprocessableEntity("Duplicate certificate!");
            }
            var CA = Certificate.CreateCACertificate(issuerName, start, end);
            var key = Certificate.TransformPrivateKey(CA);
            await context.PrivateKeys.AddAsync(key);
            await context.SaveChangesAsync();
            Certificate.LoadCACertificateToStore(CA);
            var response = Certificate.GetCACertificate(issuerName);
            if (!(response is null))
            {
                return CreatedAtAction("GetCACertificate", new { issuerName = issuerName }, response);
            }
            return Conflict();
        }

        /// <summary>
        /// Gets the  certificate with concrete name
        /// </summary>
        /// <param name="subjectName">User's name</param>
        /// <param name="issuerName">Issuer name</param>
        /// <returns>The user's certificate</returns>
        /// <response code="200">Returns the certificate with specified subject name</response>
        /// <response code="400">The subject name string was empty</response>
        /// <response code="404">Certificate with specified subject name was not found in the store</response>
        [HttpGet("/certificate/subjectName")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(X509Certificate2), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserCertificate(string subjectName, string issuerName)
        {
            if (string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(issuerName))
            {
                return BadRequest("Empty input!");
            }
            var response = Certificate.GetCertificate(subjectName, issuerName);
            if (!(response is null))
            {
                return Ok(response);
            }
            return NotFound();
        }

        /// <summary>
        /// Removes the root certificate with specified issuer name
        /// </summary>
        /// <param name="issuerName">Issuer name of the needed root certificate</param>
        /// <returns>Nothing</returns>
        /// <response code="202">The certificate has been successfully deleted</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">The root certificate with specified issuer name was not found</response>
        [HttpDelete("/certificate/issuerName")]
        [Produces("application/json")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCACertificate(string issuerName)
        {
            if (string.IsNullOrEmpty(issuerName))
            {
                return BadRequest("Invalid input!");
            }
            if (!(Certificate.GetCACertificate(issuerName) is null))
            {
                var thumbprint = Certificate.GetCACertificate(issuerName).Thumbprint;
                var response = Certificate.RemoveCACertificate(issuerName);
                context.PrivateKeys.Remove(await context.PrivateKeys.FindAsync(thumbprint));
                return Accepted();
            }
            return NotFound();
        }

        /// <summary>
        /// Removes the certificate with specified subject name
        /// </summary>
        /// <param name="subjectName">Subject name of the needed certificate</param>
        /// <param name="issuerName">Issuer name of the needed certificate</param>
        /// <returns>Nothing</returns>
        /// <response code="202">The certificate has been successfully deleted</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">The certificate with specified subject name was not found</response>
        [HttpDelete("/certificate/subjectName")]
        [Produces("application/json")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteCertificate(string subjectName, string issuerName)
        {
            if (string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(issuerName))
            {
                return BadRequest("Invalid input!");
            }
            if (!(Certificate.GetCertificate(subjectName, issuerName) is null))
            {
                var thumbprint = Certificate.GetCertificate(subjectName, issuerName).Thumbprint;
                var response = Certificate.RemoveCertificate(subjectName, issuerName);
                context.PrivateKeys.Remove(await context.PrivateKeys.FindAsync(thumbprint));
                return Accepted();
            }
            return NotFound();
        }

        /// <summary>
        /// Creates a non-root certificate with specified subject name and issuer name of the root certificate
        /// </summary>
        /// <param name="subjectName">subject name of the certificate</param>
        /// <param name="issuerName">issuer name of the certificate</param>
        /// <param name="start">certificate's start date</param>
        /// <param name="end">certificate's expiration date</param>
        /// <response code="201">Returns a newly created non-root certificate</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">Centre could not find the root certificate with specified issuer name</response>
        /// <response code="409">The certificate was created, but could not be returned at this moment</response>
        /// <response code="422">The certificate with specified parameters does exist</response>
        /// <returns></returns>
        [HttpPost("/certificate/user")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(X509Certificate2), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PostCreateCertificate(string subjectName, string issuerName, DateTimeOffset start, DateTimeOffset end)
        {
            if (string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(issuerName))
            {
                return BadRequest("Invalid input!");
            }
            if (Certificate.CertificateExists(subjectName, issuerName))
            {
                return UnprocessableEntity("The duplicate certificate with the same subject an issuer name has been found!");
            }
            var root = Certificate.GetCACertificate(issuerName);
            if (root is null)
            {
                return NotFound("Root certificate was not found!");
            }
            root = Certificate.LoadPrivateKey(root, await context.PrivateKeys.FindAsync(root.Thumbprint));
            var certificate = Certificate.CreateUserCertificate(subjectName, root, start, end);
            var key = Certificate.TransformPrivateKey(certificate);
            await context.PrivateKeys.AddAsync(key);
            await context.SaveChangesAsync();
            Certificate.LoadCertificate(certificate);
            var response = Certificate.GetCertificate(subjectName, issuerName);
            if (response is null)
            {
                return Conflict();
            }
            return CreatedAtAction("GetUserCertificate", new { subjectName = subjectName, issuerName = issuerName }, response);
        }

        /// <summary>
        /// Checks whether the certificate with specified parameters is verified in the store
        /// </summary>
        /// <param name="subjectName">subject name of needed certificate</param>
        /// <param name="issuerName">issuer name of needed certificate</param>
        /// <returns>true, if the certificate has gone through verification, otherwise false</returns>
        /// <response code="200">Returns the verification result</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">The certificate was not found</response>
        [HttpGet("/certificate/verify")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCertificateVerification(string subjectName, string issuerName)
        {
            if (string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(issuerName))
            {
                return BadRequest("Invalid input!");
            }
            var certificate = Certificate.GetCertificate(subjectName, issuerName);
            if (certificate is null)
            {
                return NotFound();
            }
            var response = Certificate.VerifyCertificate(subjectName, issuerName);
            return Ok(response);
        }

        /// <summary>
        /// Signs the data
        /// </summary>
        /// <param name="subjectName">subject name of needed certificate</param>
        /// <param name="issuerName">issuer name of needed certificate</param>
        /// <param name="data">Data to sign</param>
        /// <returns>b64 string</returns>
        /// <response code="200">Returns the signed data</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">Certificate wasn't found</response>
        /// <response code="409">Error during signing</response>
        [HttpGet("/certificate/sign")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> GetSignedData(string subjectName, string issuerName, string data)
        {
            if (string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(issuerName) || string.IsNullOrEmpty(data))
            {
                return BadRequest("Invalid input!");
            }
            if (!Certificate.CertificateExists(subjectName, issuerName))
            {
                return NotFound();
            }
            var certificate = Certificate.GetCertificate(subjectName, issuerName);
            certificate = Certificate.LoadPrivateKey(certificate, await context.PrivateKeys.FindAsync(certificate.Thumbprint));
            try
            {
                var signed = Certificate.SignHash(data, certificate);
                return Ok(signed);
            }
            catch (Exception)
            {
                return Conflict();
            }
        }

        /// <summary>
        /// Verify the signed data
        /// </summary>
        /// <param name="subjectName">subject name of needed certificate</param>
        /// <param name="issuerName">issuer name of needed certificate</param>
        /// <param name="data">Data to sign</param>
        /// <param name="info">Initial data</param>
        /// <returns>Verification result</returns>
        /// <response code="200">Returns the verification result</response>
        /// <response code="400">Invalid input</response>
        /// <response code="404">Certificate wasn't found</response>
        /// <response code="409">Error during verifying</response>
        [HttpGet("/certificate/verifyHash")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(bool), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> GetSignatureVerification(string subjectName, string issuerName, string data, string info)
        {
            if (string.IsNullOrEmpty(subjectName) || string.IsNullOrEmpty(issuerName)
                || string.IsNullOrEmpty(data) || string.IsNullOrEmpty(info))
            {
                return BadRequest("Invalid input!");
            }
            if (!Certificate.CertificateExists(subjectName, issuerName))
            {
                return NotFound();
            }
            var certificate = Certificate.GetCertificate(subjectName, issuerName);
            try
            {
                var verification = Certificate.VerifySignature(data, info, certificate);
                return Ok(verification);
            }
            catch (Exception)
            {
                return Conflict();
            }
        }
    }
}
