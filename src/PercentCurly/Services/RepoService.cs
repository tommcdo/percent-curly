using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PercentCurly.Services
{
    public class RepoService : IRepoService
    {
        private readonly HttpClient httpClient;

        public RepoService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async IAsyncEnumerable<RepoFile> GetFiles(string owner, string repo, string path = "")
        {
            var listing = await Get<RepoFile[]>($"/repos/{owner}/{repo}/contents/{path}");
            foreach (var file in listing)
            {
                if (file.Type == "file")
                {
                    yield return file;
                }
                else if (file.Type == "dir")
                {
                    await foreach (var subFile in GetFiles(owner, repo, file.Path))
                    {
                        yield return subFile;
                    }
                }
            }
        }

        private async Task<TResponse> Get<TResponse>(string path)
        {
            var response = await httpClient.GetAsync(path);
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(content);
        }
    }

    public class RepoFile
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
