using System.Collections.Generic;
using System.Threading.Tasks;

namespace PercentCurly.Services
{
    public interface IRepoService
    {
        Task<IEnumerable<Repo>> GetUserRepos(string username);
    }
}
