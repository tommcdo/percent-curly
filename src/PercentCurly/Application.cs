using System;
using System.Threading.Tasks;
using PercentCurly.Services;

namespace PercentCurly
{
    public interface IApplication
    {
        Task StartAsync();
    }

    public class Application : IApplication
    {
        private readonly IRepoService repoService;

        public Application(IRepoService repoService)
        {
            this.repoService = repoService;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Hello world!");
            var files = repoService.GetFiles("tommcdo", "kohana-twig");
            await foreach (var file in files)
            {
                Console.WriteLine($"- {file.Path}");
            }
        }
    }
}
