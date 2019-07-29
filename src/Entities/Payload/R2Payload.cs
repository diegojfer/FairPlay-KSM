using System;
using System.IO;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay.Entities.Payload
{
    sealed internal class R2Payload 
    {
        private ReadOnlyMemory<byte> Storage { get; set; }

        internal byte[] R2 { get => this.Storage.Slice(0, 21).ToArray(); }
        
        internal byte[] Binary { get => this.Storage.ToArray(); }

        internal R2Payload(ReadOnlyMemory<byte> buffer)
        {
            // ArgumentThrow.IfNull(() => buffer, "Invalid buffer length. The buffer must not be null.", nameof(buffer)); /* STRUCT CAN NOT BE NULL. */
            ArgumentThrow.IfLengthNot(buffer, 21, $"Invalid buffer length. The buffer must contains the exact number of bytes to fill entity '{this.GetType().FullName}'.", nameof(buffer));
            
            this.Storage = buffer.Slice(0, 21);
        }

        internal R2Payload(byte[] random)
        {
            ArgumentThrow.IfLengthNot(random, 21, $"Invalid random buffer length. The buffer must contains 21 bytes.", nameof(random));

            var stream = new MemoryStream();
            stream.Write(random);
            this.Storage = new ReadOnlyMemory<byte>(stream.ToArray());
        }
    }
}
