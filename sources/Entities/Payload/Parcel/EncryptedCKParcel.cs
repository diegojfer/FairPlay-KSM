using System;
using System.IO;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload.Parcel
{
    sealed internal class EncryptedCKParcel 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] Key { get => this.Storage.Slice(0, 16).ToArray(); }

        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal EncryptedCKParcel(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 16, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 16);
        }

        internal EncryptedCKParcel(byte[] key)
        {
            ArgumentThrow.IfLengthNot(key, 16, $"Invalid key buffer length. The buffer must contains 16 bytes.", nameof(key));

            var stream = new MemoryStream();
            stream.Write(key);
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }
    }
}
