using SmartElectionCertificationCentre.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SmartElectionCertificationCentre.Controllers
{
    public static class Certificate
    {
        public static X509Certificate2 CreateCACertificate(string issuerName, DateTimeOffset start, DateTimeOffset end)
        {
            using (var rsa = RSA.Create(4096))
            {
                var request = new CertificateRequest($"CN={issuerName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, true, 12, true));
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign, true));
                var sanBuilder = new SubjectAlternativeNameBuilder();
                sanBuilder.AddDnsName(issuerName);
                var sanExtension = sanBuilder.Build();
                request.CertificateExtensions.Add(sanExtension);
                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection {
                    new Oid("1.3.6.1.5.5.7.3.2"),
                    new Oid("1.3.6.1.5.5.7.3.1")
                        },
                        false));
                request.CertificateExtensions.Add(
                    new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
                return request.CreateSelfSigned(start, end);
            }
        }

        public static void LoadCACertificateToStore(X509Certificate2 CA)
        {
            using (var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser, OpenFlags.MaxAllowed))
            {
                store.Add(CA);
            }
        }

        public static bool CertificateExists(string subjectName, string issuerName)
        {
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser, OpenFlags.MaxAllowed))
            {
                var certificates = store.Certificates.Find(X509FindType.FindByIssuerName, issuerName, true).Cast<X509Certificate2>();
                return certificates.Any(x => x.Subject.Substring(3) == subjectName);
            }
        }

        public static X509Certificate2 CreateUserCertificate(string subjectName, X509Certificate2 CA, DateTimeOffset start, DateTimeOffset end)
        {
            using (var rsa = RSA.Create(4096))
            {
                var request = new CertificateRequest($"CN={subjectName}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));
                var sanBuilder = new SubjectAlternativeNameBuilder();
                sanBuilder.AddDnsName(subjectName);
                var sanExtension = sanBuilder.Build();
                request.CertificateExtensions.Add(sanExtension);
                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection {
                    new Oid("1.3.6.1.5.5.7.3.2"),
                    new Oid("1.3.6.1.5.5.7.3.1")
                        },
                        false));
                request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var unixTime = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
                var serial = BitConverter.GetBytes(unixTime);
                return request.Create(CA, start, end, serial).CopyWithPrivateKey(rsa);
            }
        }

        public static void LoadCertificate(X509Certificate2 certificate)
        {
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser, OpenFlags.MaxAllowed))
            {
                store.Add(certificate);
            }
        }

        public static X509Certificate2 GetCACertificate(string issuerName)
        {
            using (var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser, OpenFlags.MaxAllowed))
            {
                var CA = store.Certificates.Find(X509FindType.FindByIssuerName, issuerName, true);
                return CA.Count == 0 ? null : CA[0];
            }
        }

        public static X509Certificate2 GetCertificate(string subjectName, string issuerName)
        {
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser, OpenFlags.MaxAllowed))
            {
                var certificate = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true).Cast<X509Certificate2>();
                return certificate.Count() == 0 ? null : certificate.Where(x=>x.Issuer.Substring(3) == issuerName).FirstOrDefault();
            }
        }

        public static bool RemoveCACertificate(string issuerName)
        {
            using (var store = new X509Store(StoreName.Root, StoreLocation.CurrentUser, OpenFlags.MaxAllowed))
            {
                var certificates = store.Certificates.Find(X509FindType.FindByIssuerName, issuerName, true);
                if (certificates.Count == 0) return false;
                store.Remove(certificates[0]);
                return true;
            }
        }

        public static bool RemoveCertificate(string subjectName, string issuerName)
        {
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser, OpenFlags.MaxAllowed))
            {
                var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true).Cast<X509Certificate2>();
                if (certificates.Count() == 0) return false;
                var certificate = certificates.Where(x => x.Issuer.Substring(3) == issuerName).FirstOrDefault();
                if (certificate is null) return false;
                store.Remove(certificate);
                return true;
            }
        }

        public static PrivateKey TransformPrivateKey(X509Certificate2 certificate)
        {
            var key = (RSACng)certificate.PrivateKey;
            var parameters = key.ExportParameters(true);
            var model = new PrivateKey()
            {
                CertificateThumbprint = certificate.Thumbprint,
                D = Convert.ToBase64String(parameters.D),
                DP = Convert.ToBase64String(parameters.DP),
                DQ = Convert.ToBase64String(parameters.DQ),
                Exponent = Convert.ToBase64String(parameters.Exponent),
                InverseQ = Convert.ToBase64String(parameters.InverseQ),
                Modulus = Convert.ToBase64String(parameters.Modulus),
                P = Convert.ToBase64String(parameters.P),
                Q = Convert.ToBase64String(parameters.Q)
            };
            return model;
        }

        public static X509Certificate2 LoadPrivateKey(X509Certificate2 certificate, PrivateKey key)
        {
            var parameters = new RSAParameters()
            {
                D = Convert.FromBase64String(key.D),
                DP = Convert.FromBase64String(key.DP),
                DQ = Convert.FromBase64String(key.DQ),
                Exponent = Convert.FromBase64String(key.Exponent),
                InverseQ = Convert.FromBase64String(key.InverseQ),
                Modulus = Convert.FromBase64String(key.Modulus),
                P = Convert.FromBase64String(key.P),
                Q = Convert.FromBase64String(key.Q)
            };
            var rsa = new RSACng(4096);
            rsa.ImportParameters(parameters);
            return certificate.CopyWithPrivateKey(rsa);
        }

        public static bool VerifyCertificate(string subjectName, string issuerName)
        {
            using (var store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser, OpenFlags.MaxAllowed))
            {
                var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true).Cast<X509Certificate2>();
                if (certificates.Count() == 0) return false;
                var certificate = certificates.Where(x => x.Issuer.Substring(3) == issuerName).FirstOrDefault();
                if (certificate is null) return false;
                return certificate.Verify();
            }
        }

        public static string SignHash(string info, X509Certificate2 certificate)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Convert.FromBase64String(info));
                return Convert.ToBase64String(certificate.GetRSAPrivateKey().SignHash(bytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
            }
        }

        public static bool VerifySignature(string signed, string info, X509Certificate2 certificate)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Convert.FromBase64String(info));
                return certificate.GetRSAPublicKey().VerifyHash(bytes, Convert.FromBase64String(signed), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}
