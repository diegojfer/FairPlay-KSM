using System;
using System.Threading.Tasks;

namespace FoolishTech.FairPlay.CLILicenser
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            FPProvider provider = new FPProvider(Secrets.Instance.CertificateBytes, Secrets.Instance.CertificatePassphrase, Secrets.Instance.ASK);
            HardcodedKeyLocator locator = new HardcodedKeyLocator();

            FPServer licenser = new FPServer(provider, locator);

            byte[] spc = new byte[] { 0x00 };
            byte[] ckc = await licenser.GenerateCKC(spc, null);
        }
    }
}
