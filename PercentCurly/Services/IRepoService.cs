using System.Collections.Generic;
using System.Threading.Tasks;

namespace PercentCurly.Services
{
    public interface IRepoService
    {
        IAsyncEnumerable<RepoFile> GetFiles(string owner, string repo, string path = "");
    }
}
