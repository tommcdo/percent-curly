using System.IO;
using System.Threading.Tasks;
using PercentCurly.Templating;

namespace PercentCurly.FileSystem
{
    public interface IPath : ITemplate
    {
        public async Task<StreamWriter> AsStreamWriter()
        {
            var path = await GetAsync();
            return new StreamWriter(await path.ReadToEndAsync());
        }
    }
}
