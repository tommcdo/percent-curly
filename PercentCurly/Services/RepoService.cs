using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PercentCurly.Services
{
    public class RepoService : IRepoService
    {
        private readonly HttpClient httpClient;

        public RepoService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public Task<IEnumerable<Repo>> GetUserRepos(string username)
        {
            return Get<IEnumerable<Repo>>($"/users/{username}/repos");
        }

        private async Task<TResponse> Get<TResponse>(string path)
        {
            var response = await httpClient.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<TResponse>(content);
        }
    }

    public class Repo
    {
        public string Name { get; set; }
    }
}
