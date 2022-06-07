using System;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Sophie.Areas.Admin.IdentityHostingStartup))]
namespace Sophie.Areas.Admin
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