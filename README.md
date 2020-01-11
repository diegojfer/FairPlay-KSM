# FairPlay - Key Security Module 


## What is FairPlay-KSM?

FairPlay-KSM is a NETCore implementation of Apple DRM protocol. According to Apple, FairPlay Streaming (FPS) securely delivers keys to Apple mobile devices, Apple TV, and Safari on macOS, which will enable playback of encrypted video content.

This implementation can be used as license expeditor or spc-ckc debugger (partially implemented). 

## Where can I download the library?

![NuGet](https://www.nuget.org/Content/gallery/img/logo-header.svg)

FairPlay has been packaged as [NuGet package](https://www.nuget.org/packages/FoolishTech.FairPlay/), so you only have to include the `FoolishTech.FairPlay` package in your project.

`dotnet add package FoolishTech.FairPlay`

## How can I use the library?

Before using the module, you must [Request Deployment Package](https://developer.apple.com/contact/fps/) to Apple. 

After that, you can run your own HTTP license server. Check-out our [examples](https://github.com/diegojfer/FairPlay-KSM/tree/master/examples).

### Very simple License Expeditor

```csharp
using  System;
using  System.Text;
using  System.Threading.Tasks;
using  FoolishTech.FairPlay;
using  FoolishTech.FairPlay.Models;
using  FoolishTech.FairPlay.Interfaces;
using  FoolishTech.FairPlay.Exceptions;

namespace FoolishTech.SimpleExpeditor
{
	public class HardcodedKeyLocator: IContentKeyLocator
	{
		Task<IContentKey> IContentKeyLocator.FetchContentKey(byte[] contentId, object  info /* Object passed on GenerateCKC */)
		{
			string  id = Encoding.UTF8.GetString(contentId);

			if (id.Equals("twelve")) return  Task.FromResult<IContentKey>(new  FPStaticKey("3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C3C", "D5FBD6B82ED93E4EF98AE40931EE33B7"));
			else  throw  new  ArgumentOutOfRangeException(nameof(contentId), $"We can't find key for content id ${contentId}");
		}
	}
	
	public class SimpleFairPlay
	{
		public async Task<byte[]> Resolve(byte[] spc)
		{
			try {
				FPProvider  provider = new  FPProvider(new  byte[] { /* Certificate+PrivKey P12 */ }, ""  /* P12 Passphrase */, new  byte[] { /* ASK */ });
				IContentKeyLocator  locator = new  HardcodedKeyLocator();

				FPServer  server = new  FPServer(provider, locator);
				return  await  server.GenerateCKC(spc, new  Object());
			} catch (FPKeyLocatorException) {
				// Exception throwed on IContentKeyLocator
			} catch (Exception) {
				// Any other exception.
			}
			
			return  null;
		}
	}
}
```

## Can I contribute to FairPlay-KSM?

Yes! Open pull request. :D

## Authors

Diego Fernández - [me@diegofer.com](mailto:me@diegofer.com) 
