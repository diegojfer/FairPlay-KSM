namespace FoolishTech.FairPlay.Interfaces
{
    public interface IContentKey
    {
        byte[] Key { get; }
    
        byte[] IV { get; }
    }
}