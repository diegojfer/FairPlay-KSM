using System;
using System.Text;
using System.Threading.Tasks;
using FoolishTech.FairPlay.Models;
using FoolishTech.FairPlay.Interfaces;

namespace FoolishTech.FairPlay.CLILicenser
{
    public class HardcodedKeyLocator: IContentKeyLocator
    {
        Task<IContentKey> IContentKeyLocator.FetchContentKey(byte[] contentId, object info)
        {   
            string id = Encoding.UTF8.GetString(contentId);

            if (id.Equals("twelve")) return Task.FromResult<IContentKey>(new FPStaticKey("3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C", "D5FBD6B82ED93E4EF98AE40931EE33B7"));
            else throw new ArgumentOutOfRangeException(nameof(contentId), $"We can't find key for content id ${contentId}");
        }
    }
}
