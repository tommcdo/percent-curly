using System.IO;
using System.Threading.Tasks;

namespace PercentCurly.FileSystem
{
    public interface ITemplate
    {
        Task<StreamReader> GetAsync();
    }
}
