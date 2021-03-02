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
            var repos = repoService.GetFiles("tommcdo", "kohana-twig");
            await foreach (var repo in repos)
            {
                Console.WriteLine($"- {repo.Path}");
            }
        }
    }
}
