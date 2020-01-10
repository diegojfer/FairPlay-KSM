using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using FoolishTech.FairPlay.HTTPLicenser;
using FoolishTech.FairPlay.HTTPLicenser.Entities;
using FoolishTech.FairPlay.Exceptions;

namespace FoolishTech.FairPlay.HTTPLicenser.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FairPlayController : ControllerBase
    {
        private readonly ILogger<FairPlayController> _logger;
        private readonly FPServer _server;
        private readonly FPProvider _provider;

        public FairPlayController(ILogger<FairPlayController> logger, FPServer server, FPProvider provider)
        {
            _logger = logger;
            _server = server;
            _provider = provider;
        }

        [HttpGet]
        public Task<CertificateResponseDto> FetchCertificate()
        {
            return Task.FromResult<CertificateResponseDto>(new CertificateResponseDto() {
                Certificate = Convert.ToBase64String(_provider.Certificate)
            });
        }

        [HttpPost]
        public async Task<IActionResult> GenerateLicense([FromBody] LicenseRequestDto request)
        {
            try {
                byte[] spc = Convert.FromBase64String(request.SpcMessage);
                var ckc = await _server.GenerateCKC(spc);

                return StatusCode(500, new LicenseResponseDto() {
                    CkcMessage = Convert.ToBase64String(ckc)
                });
            } catch (FPKeyLocatorException) {
                return StatusCode(500, new LicenseResponseDto() {
                    CkcMessage = "KEY LOCATOR ERROR"
                });
            } catch (Exception) {
                return StatusCode(500, new LicenseResponseDto() {
                    CkcMessage = "INTERNAL ERROR"
                });
            }
        }
    }
}
