using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

using FoolishTech.FairPlay.Crypto;
using FoolishTech.FairPlay.Entities;
using FoolishTech.FairPlay.Entities.Payload;
using FoolishTech.FairPlay.Entities.Payload.Parcel;
using FoolishTech.FairPlay.Models;
using FoolishTech.FairPlay.Exceptions;
using FoolishTech.FairPlay.Interfaces;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay
{
    public sealed class FPExtractor
    {
        private IEnumerable<FPProvider> Providers { get; set; }

        public FPExtractor(FPProvider provider)
        {
            ArgumentThrow.IfNull(provider, "Unable to create inspector. Provider can not be null.", nameof(provider));

            this.Providers = new List<FPProvider>() { provider };
        }

        public byte[] FetchId(byte[] spc, byte[] ckc)
        {
            var spcBuffer = new ReadOnlyMemory<byte>(spc);
            var ckcBuffer = new ReadOnlyMemory<byte>(ckc);

            // We try to read SPC message. 
            SPCMessage request = null;
            try { request = new SPCMessage(spcBuffer); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2041", "Unable to parse SPC. See 'innerException' for more information.", ex); }

            CKCMessage response = null;
            try { response = new CKCMessage(ckcBuffer); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2042", "Unable to parse CKC. See 'innerException' for more information.", ex); }

            FPProvider provider = null;
            try { provider = this.Providers.First(p => Enumerable.SequenceEqual(p.Hash, request.ProviderHash)); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidProviderException("EXC_2043", "Received SPCMessage contains a unregistered FairPlay provider.", ex); }

            return FetchId(provider, request, response);
        }

        public IContentKey FetchKey(byte[] spc, byte[] ckc)
        {
            var spcBuffer = new ReadOnlyMemory<byte>(spc);
            var ckcBuffer = new ReadOnlyMemory<byte>(ckc);

            // We try to read SPC message. 
            SPCMessage request = null;
            try { request = new SPCMessage(spcBuffer); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2001", "Unable to parse SPC. See 'innerException' for more information.", ex); }

            CKCMessage response = null;
            try { response = new CKCMessage(ckcBuffer); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2002", "Unable to parse CKC. See 'innerException' for more information.", ex); }

            FPProvider provider = null;
            try { provider = this.Providers.First(p => Enumerable.SequenceEqual(p.Hash, request.ProviderHash)); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidProviderException("EXC_2003", "Received SPCMessage contains a unregistered FairPlay provider.", ex); }

            return FetchKey(provider, request, response);
        }

        private byte[] FetchId(FPProvider provider, SPCMessage spc, CKCMessage ckc) 
        {
            IEnumerable<TLLVSlab> spcSlabs = null;
            try { spcSlabs = TLLVSlab.FromBuffer(spc.UnscrambledPayload(provider.RSAKey)); } 
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2030", "Unable to process inspection. See 'innerException' for more information.", ex); }

            AssetPayload AssetPayload = null;
            try { AssetPayload = spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.Asset)).Payload<AssetPayload>(); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2031", "Unable to process inspection. See 'innerException' for more information.", ex); }

            byte[] ID = null;
            try { ID = AssetPayload.Identifier; }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2032", "Unable to process inspection. See 'innerException' for more information.", ex); }

            return ID;
        }

        private IContentKey FetchKey(FPProvider provider, SPCMessage spc, CKCMessage ckc) 
        {
            IEnumerable<TLLVSlab> spcSlabs = null;
            try { spcSlabs = TLLVSlab.FromBuffer(spc.UnscrambledPayload(provider.RSAKey)); } 
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2010", "Unable to process inspection. See 'innerException' for more information.", ex); }

            byte[] DASk = null;
            try { DASk = new DFunction(provider.ASKey).DeriveBytes(spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.R2)).Payload<R2Payload>().R2); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2011", "Unable to process inspection. See 'innerException' for more information.", ex); }

            SKR1Payload SessionRandom1Payload = null;
            try { SessionRandom1Payload = spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.SKR1)).Payload<SKR1Payload>(); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2012", "Unable to process inspection. See 'innerException' for more information.", ex); }

            SKR1IntegrityPayload SessionRandom1IntegrityPayload = null;
            try { SessionRandom1IntegrityPayload = spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.SKR1Integrity)).Payload<SKR1IntegrityPayload>(); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2013", "Unable to process inspection. See 'innerException' for more information.", ex); }

            SKR1Parcel SessionRandom1Parcel = null;
            try { SessionRandom1Parcel = new SKR1Parcel(new ReadOnlyMemory<byte>(SessionRandom1Payload.UnscrambledParcel(DASk))); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2014", "Unable to process inspection. See 'innerException' for more information.", ex); }

            if (Enumerable.SequenceEqual(SessionRandom1Parcel.Integrity, SessionRandom1IntegrityPayload.Integrity) == false) throw new FPContextIntegrityViolatedException("EXC_0034", "Unable to process SPC. SPC integrity violated.");

            byte[] seed = null;
            try { seed = spcSlabs.First(s => s.Tag.Same(TLLVTag.Id.AR)).Payload<ARPayload>().Seed; }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2015", "Unable to process inspection. See 'innerException' for more information.", ex); }
            
            byte[] random = null;
            try { random = SessionRandom1Parcel.R1; }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2016", "Unable to process inspection. See 'innerException' for more information.", ex); }

            byte[] key = null;
            try {
                using (var sha = SHA1.Create())
                using (var aes = Aes.Create())
                {
                    // aes.IV = new byte[16];
                    aes.Key = sha.ComputeHash(random).Take(16).ToArray();
                    aes.Mode = CipherMode.ECB;
                    aes.Padding = PaddingMode.None;

                    using (var encryptor = aes.CreateEncryptor())
                    using (var memoryStream = new MemoryStream())
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(seed);

                        key = memoryStream.ToArray();
                    }    
                }
            }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2017", "Unable to process inspection. See 'innerException' for more information.", ex); }

            IEnumerable<TLLVSlab> ckcSlabs = null;
            try { ckcSlabs = TLLVSlab.FromBuffer(ckc.UnscrambledPayload(key)); } 
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2018", "Unable to process inspection. See 'innerException' for more information.", ex); }

            byte[] SK = null;
            try { SK = SessionRandom1Parcel.SK; }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2019", "Unable to process inspection. See 'innerException' for more information.", ex); }

            return FetchKey(spcSlabs, ckcSlabs, SK);
        }

        private IContentKey FetchKey(IEnumerable<TLLVSlab> spcSlabs, IEnumerable<TLLVSlab> ckcSlabs, byte[] SK) 
        {
            EncryptedCKPayload EncryptedContentKeyPayload = null;
            try { EncryptedContentKeyPayload = ckcSlabs.First(s => s.Tag.Same(TLLVTag.Id.EncryptedCK)).Payload<EncryptedCKPayload>(); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2020", "Unable to process inspection. See 'innerException' for more information.", ex); }

            EncryptedCKParcel EncryptedContentKeyParcel = null;
            try {EncryptedContentKeyParcel = new EncryptedCKParcel(new ReadOnlyMemory<byte>(EncryptedContentKeyPayload.UnscrambledParcel(SK))); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2021", "Unable to process inspection. See 'innerException' for more information.", ex); }

            byte[] IV = null;
            try { IV = EncryptedContentKeyPayload.IV; }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2022", "Unable to process inspection. See 'innerException' for more information.", ex); }

            byte[] CK = null;
            try { CK = EncryptedContentKeyParcel.Key; }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_2023", "Unable to process inspection. See 'innerException' for more information.", ex); }

            return new FPStaticKey(CK, IV);
        }
    }
}
