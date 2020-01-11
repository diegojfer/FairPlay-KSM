using System;
using FoolishTech.FairPlay.Interfaces;


namespace FoolishTech.FairPlay.SPCCKCSniffer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try {
                byte[] spc = Convert.FromBase64String("");
                byte[] ckc = Convert.FromBase64String("");

                FPProvider provider = new FPProvider(Secrets.Instance.CertificateBytes, Secrets.Instance.CertificatePassphrase, Secrets.Instance.ASK);

                FPExtractor inspector = new FPExtractor(provider);

                byte[] identifier = inspector.FetchId(spc, ckc);
                IContentKey key = inspector.FetchKey(spc, ckc);

                Console.WriteLine($"SUCCESS: [ID: {Convert.ToBase64String(identifier)}, KEY: {Convert.ToBase64String(key.Key)}, IV: {Convert.ToBase64String(key.IV)}]");
            } catch (Exception ex) {
                Console.WriteLine($"EXCEPTION: {ex.Message}");
            }

        }
    }
}
