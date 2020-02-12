using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace BlackAndWhiteToColor
{
    public static class ConfigurationHelper
    {
        private static string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        private static IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.{environmentName}.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

        public static IConfigurationRoot Configuration = builder.Build();
    }
}