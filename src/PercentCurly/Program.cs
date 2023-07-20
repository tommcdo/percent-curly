using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using PercentCurly.Services;

namespace PercentCurly
{
    class Program
    {
        static void ConfigureServices(ServiceCollection services)
        {
            services.AddHttpClient<IRepoService, RepoService>(builder =>
            {
                builder.BaseAddress = new Uri("https://api.github.com/");
                builder.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
                builder.DefaultRequestHeaders.Add("User-Agent", "percent-curly");
            });
            services.AddSingleton<IApplication, Application>();
        }

        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            await services.BuildServiceProvider()
                .GetRequiredService<IApplication>()
                .StartAsync();
        }
    }
}
