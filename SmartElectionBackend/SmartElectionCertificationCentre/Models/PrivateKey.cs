using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartElectionCertificationCentre.Models
{
    public class PrivateKey
    {
        [Required]
        public string CertificateThumbprint { get; set; }
        [Required]
        public string D { get; set; }
        [Required]
        public string DP { get; set; }
        [Required]
        public string DQ { get; set; }
        [Required]
        public string Exponent { get; set; }
        [Required]
        public string InverseQ { get; set; }
        [Required]
        public string Modulus { get; set; }
        [Required]
        public string P { get; set; }
        [Required]
        public string Q { get; set; }
    }
}
