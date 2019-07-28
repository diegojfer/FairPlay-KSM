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

        public FPProvider(X509Certificate certificate, RSAParameters key, byte[] ask)
        {
            ArgumentThrow.IfNull(certificate, "Invalid FairPlay Certificate. Certificate can not be null.", nameof(certificate));
            ArgumentThrow.IfNull(key, "Invalid FairPlay Certificate Key. RSAParameters can not be null.", nameof(key));
            ArgumentThrow.IfLengthNot(ask, 16, "Invalid Application Secret Key. ASK buffer should be 16 bytes.", nameof(ask));

            this.Certificate = certificate;
            this.RSAKey = key;
            this.ASKey = ask;
        }
    }
}
