using System.IO;
using System.Threading.Tasks;

namespace PercentCurly.FileSystem
{
    public class File
    {
        private readonly IPath _path;
        private readonly IBuffer _buffer;

        public File(IPath path, IBuffer buffer)
        {
            _path = path;
            _buffer = buffer;
        }

        public async Task WriteAsync()
        {
            var file = await _path.AsStreamWriter();
            var buffer = await _buffer.GetAsync();
            string line = await buffer.ReadLineAsync();
            while (line != null)
            {
                await file.WriteLineAsync(line);
                line = await buffer.ReadLineAsync();
            }
        }
    }
}
