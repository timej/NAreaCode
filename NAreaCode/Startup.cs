using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;

namespace NAreaCode
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }
        public Startup(ApplicationEnvironment env)
        {
            string os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" : "linux";
            var builder = new ConfigurationBuilder();
            builder
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{os}.json", optional: true);

            Configuration = builder.Build();
        }
    }
}
