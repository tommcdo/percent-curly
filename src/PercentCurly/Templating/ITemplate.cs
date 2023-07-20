using System.IO;
using System.Threading.Tasks;

namespace PercentCurly.Templating
{
    public interface ITemplate
    {
        Task<StreamReader> GetAsync();
    }
}
