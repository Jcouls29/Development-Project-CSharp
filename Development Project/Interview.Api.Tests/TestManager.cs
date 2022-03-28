using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace Interview.Api.Tests
{
    public class TestManager
    {
        private IConfiguration _configuration;
        public TestManager()
        {
            var services=new ServiceCollection().AddSingleton<IConfiguration>(Configuration).AddLogging().BuildServiceProvider();
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
#pragma warning disable CS8601 // Possible null reference assignment.
            LoggerFactory = services.GetService<ILoggerFactory>();
#pragma warning restore CS8601 // Possible null reference assignment.
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
        }
        public ILoggerFactory LoggerFactory { get; }
        public IConfiguration Configuration
        {
            get
            {
                if(_configuration == null)
                {
                    string[] paths = { Directory.GetCurrentDirectory(), "appsettings.json" };
                    var builder = new ConfigurationBuilder().AddJsonFile(Path.Combine(paths), optional: true).AddEnvironmentVariables();
                    _configuration = builder.Build();
                }
                return _configuration;
            }
        }
    }
}