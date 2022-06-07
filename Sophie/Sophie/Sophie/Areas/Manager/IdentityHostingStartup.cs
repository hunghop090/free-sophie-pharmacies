using System;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Sophie.Areas.Manager.IdentityHostingStartup))]
namespace Sophie.Areas.Manager
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}