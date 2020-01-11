using System;

namespace FoolishTech.FairPlay.HTTPLicenser
{
    public class Secrets
    {
        private static Secrets _instance = new Secrets();
        public static Secrets Instance { get => _instance; } 

        private Secrets() { }

        // DON'T STORE SECRET HERE, 
        // instead pass them as ENV. 
        public byte[] CertificateBytes { get => Convert.FromBase64String(""); }
        public string CertificatePassphrase { get => ""; }
        public byte[] ASK { get => Convert.FromBase64String(""); }
    }
}
