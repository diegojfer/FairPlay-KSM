using System;
using System.IO;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class AssetPayload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] Identifier { get => this.Storage.ToArray(); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal AssetPayload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            
            this.Storage = buffer;
        }

        internal AssetPayload(byte[] identifier)
        {
            ArgumentThrow.IfLackingBytes(identifier, 1, "Invalid identifier buffer length. The buffer must contains more than 1 bytes.", nameof(identifier));

            var stream = new MemoryStream();
            stream.Write(identifier);
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }
    }
}
