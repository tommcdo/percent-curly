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
        private readonly IRepoService _repoService;

        public Application(IRepoService repoService)
        {
            _repoService = repoService;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Hello world!");
            var repos = await _repoService.GetUserRepos("tommcdo");
            foreach (var repo in repos)
            {
                Console.WriteLine($"- {repo.Name}");
            }
        }
    }
}
