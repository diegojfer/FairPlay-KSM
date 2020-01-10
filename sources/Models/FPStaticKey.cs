using System;
using System.Linq;
using FoolishTech.FairPlay.Interfaces;

namespace FoolishTech.FairPlay.Models
{
    public class FPStaticKey: IContentKey
    {
        public byte[] Key { get; private set; }
    
        public byte[] IV { get; private set; }

        public FPStaticKey(string hexKey, string hexIv) {
            this.Key = Enumerable.Range(0, hexKey.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hexKey.Substring(x, 2), 16)).ToArray();
            this.IV = Enumerable.Range(0, hexIv.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(hexIv.Substring(x, 2), 16)).ToArray();
        }

        public FPStaticKey(byte[] key, byte[] iv) {
            this.Key = key;
            this.IV = iv;
        }
    }
}
