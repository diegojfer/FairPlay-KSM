using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;

using FoolishTech.FairPlay.Crypto;
using FoolishTech.FairPlay.Entities;
using FoolishTech.FairPlay.Entities.Payload;
using FoolishTech.FairPlay.Entities.Payload.Parcel;
using FoolishTech.FairPlay.Exceptions;
using FoolishTech.FairPlay.Interfaces;
using FoolishTech.Support.Throws;

namespace FoolishTech.FairPlay
{
    public sealed class FPServer
    {
        private IEnumerable<FPProvider> Providers { get; set; }
        private IContentKeyLocator Locator { get; set; }

        public int LicenseDuration { get; set; }

        public FPServer(FPProvider provider, IContentKeyLocator locator)
        {
            ArgumentThrow.IfNull(provider, "Unable to create server. Provider can not be null.", nameof(provider));
            ArgumentThrow.IfNull(locator, "Unable to create server. Key locator can not be null.", nameof(locator));

            this.Providers = new List<FPProvider>() { provider };
            this.Locator = locator;

            this.LicenseDuration = 3600;
        }

        public async Task<byte[]> GenerateCKC(byte[] spc, object info = null)
        {
            var buffer = new ReadOnlyMemory<byte>(spc);

            // We try to read SPC message. 
            SPCMessage request = null;
            try { request = new SPCMessage(buffer); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0001", "Unable to parse SPC. See 'innerException' for more information.", ex); }

            FPProvider provider = null;
            try { provider = this.Providers.First(p => Enumerable.SequenceEqual(p.Hash, request.ProviderHash)); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidProviderException("EXC_0002", "Received SPCMessage contains a unregistered FairPlay provider.", ex); }

            CKCMessage response = null;
            try { response = await GenerateCKC(provider, request, info); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0003", "Unable to process SPC. See 'innerException' for more information.", ex); }

            return response.Binary;
        }

        private async Task<CKCMessage> GenerateCKC(FPProvider provider, SPCMessage spc, object info)
        {
            if (spc == null) throw new FPInvalidContextException("EXC_0004", "Null SPCMessage received. Unable to process request.");
            if (provider == null) throw new FPInvalidProviderException("EXC_0005", "Null FPProvider received. Unable to process request.");
            if (Enumerable.SequenceEqual(provider.Hash, spc.ProviderHash) == false) throw new FPInvalidProviderException("EXC_0006", "Received SPCMessage contains a unregistered FairPlay provider.");

            IEnumerable<TLLVSlab> spcSlabs = null;
            try { spcSlabs = TLLVSlab.FromBuffer(spc.UnscrambledPayload(provider.RSAKey)); } 
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0007", "Unable to process SPC. See 'innerException' for more information.", ex); }

            IEnumerable<TLLVSlab> ckcSlabs = null;
            try { ckcSlabs = await GenerateCKC(new DFunction(provider.ASKey), spcSlabs, info); } 
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0008", "Unable to process CKC. See 'innerException' for more information.", ex); }

            byte[] seed = null;
            try { seed = spcSlabs.First(s => s.Tag.Same(TLLVTag.Id.AR)).Payload<ARPayload>().Seed; }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0009", "Unable to process SPC. See 'innerException' for more information.", ex); }
            
            byte[] random = null;
            try { random = ckcSlabs.First(s => s.Tag.Same(TLLVTag.Id.R1)).Payload<R1Payload>().R1; }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0010", "Unable to process SPC. See 'innerException' for more information.", ex); }
            
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
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0011", "Unable to process CKC. See 'innerException' for more information.", ex); }
            

            byte[] iv = new byte[16];
            try { new Random().NextBytes(iv); } 
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0012", "Unable to process CKC. See 'innerException' for more information.", ex); }

            byte[] ckc = null;
            try { ckc = TLLVCrypto.ScrambledPayload(TLLVSlab.ToBuffer(ckcSlabs), iv, key); } 
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0013", "Unable to process CKC. See 'innerException' for more information.", ex); }

            return new CKCMessage(1, iv, ckc);
        }

        private async Task<IEnumerable<TLLVSlab>> GenerateCKC(DFunction derivator, IEnumerable<TLLVSlab> spcSlabs, object info) 
        {
            var ckcSlabs = new List<TLLVSlab>(); 

            byte[] DASk = null;
            try { DASk = derivator.DeriveBytes(spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.R2)).Payload<R2Payload>().R2); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0014", "Unable to process SPC. See 'innerException' for more information.", ex); }

            SKR1Payload SessionRandom1Payload = null;
            try { SessionRandom1Payload = spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.SKR1)).Payload<SKR1Payload>(); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0015", "Unable to process SPC. See 'innerException' for more information.", ex); }

            SKR1IntegrityPayload SessionRandom1IntegrityPayload = null;
            try { SessionRandom1IntegrityPayload = spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.SKR1Integrity)).Payload<SKR1IntegrityPayload>(); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0016", "Unable to process SPC. See 'innerException' for more information.", ex); }

            SKR1Parcel SessionRandom1Parcel = null;
            try { SessionRandom1Parcel = new SKR1Parcel(new ReadOnlyMemory<byte>(SessionRandom1Payload.UnscrambledParcel(DASk))); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0017", "Unable to process SPC. See 'innerException' for more information.", ex); }

            if (Enumerable.SequenceEqual(SessionRandom1Parcel.Integrity, SessionRandom1IntegrityPayload.Integrity) == false) throw new FPContextIntegrityViolatedException("EXC_0034", "Unable to process SPC. SPC integrity violated.");

            TRRPayload TagReturnRequestPayload = null;
            try { TagReturnRequestPayload = spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.TRR)).Payload<TRRPayload>(); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0018", "Unable to process SPC. See 'innerException' for more information.", ex); }

            ckcSlabs.AddRange(spcSlabs.Where(slab => TagReturnRequestPayload.TRR.Select(tag => tag.Code).Contains(slab.Tag.Code)));
            
            AssetPayload AssetIdentifierPayload = null;
            try { AssetIdentifierPayload = spcSlabs.First(p => p.Tag.Same(TLLVTag.Id.Asset)).Payload<AssetPayload>(); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0019", "Unable to process SPC. See 'innerException' for more information.", ex); }

            IContentKey ContentKey = null;
            try { ContentKey = await this.Locator.FetchContentKey(AssetIdentifierPayload.Identifier, info); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPKeyLocatorException("EXC_0020", "Unable to fetch content key from key locator. See 'innerException' for more information.", ex); }
            
            if (ContentKey.IV == null || ContentKey.IV.Length != 16) throw new FPInvalidKeyException("EXC_0035", "Invalid IV fetched from key locator. IV can not be null and should be 16 bytes length.");
            if (ContentKey.Key == null || ContentKey.Key.Length != 16) throw new FPInvalidKeyException("EXC_0036", "Invalid key fetched from key locator. Key can not be null and should be 16 bytes length.");
            
            EncryptedCKParcel EncryptedContentKeyParcel = null; 
            try { EncryptedContentKeyParcel = new EncryptedCKParcel(ContentKey.Key); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0021", "Unable to process SPC. See 'innerException' for more information.", ex); }

            EncryptedCKPayload EncryptedContentKeyPayload = null;
            try { EncryptedContentKeyPayload = new EncryptedCKPayload(ContentKey.IV, EncryptedContentKeyParcel.ScrambledParcel(SessionRandom1Parcel.SK)); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0022", "Unable to process SPC. See 'innerException' for more information.", ex); }

            TLLVSlab EncryptedContentKeySlab = null;
            try { EncryptedContentKeySlab = new TLLVSlab(TLLVTag.Id.EncryptedCK, EncryptedContentKeyPayload.Binary); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0023", "Unable to process SPC. See 'innerException' for more information.", ex); }

            ckcSlabs.Add(EncryptedContentKeySlab);

            R1Payload Random1Payload = null;
            try {Random1Payload = new R1Payload(SessionRandom1Parcel.R1); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0024", "Unable to process SPC. See 'innerException' for more information.", ex); }

            TLLVSlab Random1Slab = null;
            try { Random1Slab = new TLLVSlab(TLLVTag.Id.R1, Random1Payload.Binary); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0025", "Unable to process SPC. See 'innerException' for more information.", ex); }

            ckcSlabs.Add(Random1Slab);

            TLLVSlab MediaPlaybackSlab = null;
            try { MediaPlaybackSlab = spcSlabs.FirstOrDefault(p => p.Tag.Same(TLLVTag.Id.MediaPlayback)); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0026", "Unable to process SPC. See 'innerException' for more information.", ex); }
            
            if (MediaPlaybackSlab != null) 
            {
                MediaPlaybackPayload MediaPlaybackPayload = null;
                try { MediaPlaybackPayload = MediaPlaybackSlab.Payload<MediaPlaybackPayload>(); }
                catch (FPException) { throw; }
                catch (Exception ex) { throw new FPInvalidContextException("EXC_0027", "Unable to process SPC. See 'innerException' for more information.", ex); }
            
                if (MediaPlaybackPayload.CreationDate < DateTime.UtcNow.AddHours(-6)) throw new FPContextDateViolatedException("EXC_0036", "Unable to process SPC. Date range violated.");
                if (MediaPlaybackPayload.CreationDate > DateTime.UtcNow.AddHours(6))  throw new FPContextDateViolatedException("EXC_0037", "Unable to process SPC. Date range violated.");
                
                DurationCKPayload DurationContentKeyPayload = null;
                try { DurationContentKeyPayload = new DurationCKPayload((UInt32)this.LicenseDuration, 0, DurationCKType.Rental); }
                catch (FPException) { throw; }
                catch (Exception ex) { throw new FPInvalidContextException("EXC_0028", "Unable to process SPC. See 'innerException' for more information.", ex); }
            
                TLLVSlab DurationContentKeySlab = null;
                try { DurationContentKeySlab = new TLLVSlab(TLLVTag.Id.DurationCK, DurationContentKeyPayload.Binary); }
                catch (FPException) { throw; }
                catch (Exception ex) { throw new FPInvalidContextException("EXC_0029", "Unable to process SPC. See 'innerException' for more information.", ex); }

                ckcSlabs.Add(DurationContentKeySlab);
            }

            TLLVSlab CapabilitiesSlab = null;
            try { CapabilitiesSlab = spcSlabs.FirstOrDefault(p => p.Tag.Same(TLLVTag.Id.Capabilities)); }
            catch (FPException) { throw; }
            catch (Exception ex) { throw new FPInvalidContextException("EXC_0030", "Unable to process SPC. See 'innerException' for more information.", ex); }
            
            if (CapabilitiesSlab != null)
            {
                // Send HDCPEnforcementSlab
                CapabilitiesPayload CapabilitiesPayload = null;
                try { CapabilitiesPayload = CapabilitiesSlab.Payload<CapabilitiesPayload>(); }
                catch (FPException) { throw; }
                catch (Exception ex) { throw new FPInvalidContextException("EXC_0031", "Unable to process SPC. See 'innerException' for more information.", ex); }
            
                if (CapabilitiesPayload.HasCapability(LowCapabilities.HDCPEnforcement))
                {
                    HDCPEnforcementPayload HDCPEnforcementPayload = null;
                    try { HDCPEnforcementPayload = new HDCPEnforcementPayload(HDCPTypeRequierement.HDCPType0Required); }
                    catch (FPException) { throw; }
                    catch (Exception ex) { throw new FPInvalidContextException("EXC_0032", "Unable to process SPC. See 'innerException' for more information.", ex); }

                    TLLVSlab HDCPEnforcementSlab = null;
                    try { HDCPEnforcementSlab = new TLLVSlab(TLLVTag.Id.HDCPEnforcement, HDCPEnforcementPayload.Binary); }
                    catch (FPException) { throw; }
                    catch (Exception ex) { throw new FPInvalidContextException("EXC_0033", "Unable to process SPC. See 'innerException' for more information.", ex); }

                    ckcSlabs.Add(HDCPEnforcementSlab);
                }
            }

            return ckcSlabs;
        }
    }
}
