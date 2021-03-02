using System.IO;
using System.Threading.Tasks;

namespace PercentCurly.FileSystem
{
    public class File
    {
        private readonly IPath path;
        private readonly IBuffer buffer;

        public File(IPath path, IBuffer buffer)
        {
            this.path = path;
            this.buffer = buffer;
        }

        public async Task WriteAsync()
        {
            var file = await path.AsStreamWriter();
            var buffer = await this.buffer.GetAsync();
            string line = await buffer.ReadLineAsync();
            while (line != null)
            {
                await file.WriteLineAsync(line);
                line = await buffer.ReadLineAsync();
            }
        }
    }
}
