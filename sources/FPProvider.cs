using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay
{
    public sealed class FPProvider
    {
        internal X509Certificate Certificate { get; private set; }
        internal RSAParameters RSAKey { get; private set; }
        internal byte[] ASKey { get; private set; } 

        public string Identifier { get => this.Certificate.GetCertHashString(); }
        public string Name { get => this.Certificate.Subject; }
        public byte[] Hash { get => this.Certificate.GetCertHash(); }

        public FPProvider(byte[] certificate, string password, byte[] ask) 
        {
            ArgumentThrow.IfNull(certificate, "Invalid FairPlay Certificate. Certificate can not be null.", nameof(certificate));
            ArgumentThrow.IfNull(password, "Invalid FairPlay Certificate passphrase. Passphrase can not be null.", nameof(password));
            ArgumentThrow.IfLengthNot(ask, 16, "Invalid Application Secret Key. ASK buffer should be 16 bytes.", nameof(ask));

            try {
                X509Certificate2 X509 = new X509Certificate2(certificate, password, X509KeyStorageFlags.Exportable);

                using (var rsa = X509.GetRSAPrivateKey()) {
                    this.Certificate = X509;
                    this.RSAKey = rsa.ExportParameters(true);
                    this.ASKey = ask;
                }
            } catch (Exception ex) {
                throw new ArgumentException("Invalid FairPlay Certificate. Certificate can not be read.", nameof(certificate), ex);
            }
        }
    }
}
