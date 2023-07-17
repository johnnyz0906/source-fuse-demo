using JohnnyDemo.Api.Options;
using JohnnyDemo.Repository.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace JohnnyDemo.IntegrationTests
{
    internal static class Helpers
    {
        private static WebApplicationFactory<Program>? _application;

        private static void ConfigureServices(IServiceCollection services, HashSet<string> apiKeys)
        {
            services.AddDbContext<JohnnyDemoContext>(option => option.UseInMemoryDatabase("test-database"));
            services.Configure<AuthOptions>(x =>
            {
                x.ApiKeys = apiKeys;
            });
        }

        internal static HttpClient GetClient(HashSet<string> apiKeys)
        {
            _application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        ConfigureServices(services, apiKeys);
                    });
                });
            return _application.CreateDefaultClient();
        }
    }
}
