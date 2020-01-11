using System.Threading.Tasks;

namespace FoolishTech.FairPlay.Interfaces
{
    public interface IContentKeyLocator
    {
        Task<IContentKey> FetchContentKey(byte[] contentId, object info);
    }
}