
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using FoolishTech.FairPlay;
using FoolishTech.FairPlay.Interfaces;

using FoolishTech.FairPlay.HTTPLicenser;

namespace FoolishTech.FairPlay.HTTPLicenser
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<IContentKeyLocator, HardcodedKeyLocator>();
            services.AddSingleton<FPProvider>(new FPProvider(Secrets.Instance.CertificateBytes, Secrets.Instance.CertificatePassphrase, Secrets.Instance.ASK));
            services.AddSingleton<FPServer, FPServer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
