using System.IO;
using System.Threading.Tasks;
using PercentCurly.Templating;

namespace PercentCurly.FileSystem;

public interface IBuffer : ITemplate
{
}

public class Buffer : IBuffer
{
    private readonly Stream stream;
    private readonly IPlaceholderResolver placeholderResolver;

    public Buffer(Stream stream, IPlaceholderResolver placeholderResolver)
    {
        this.stream = stream;
        this.placeholderResolver = placeholderResolver;
    }

    public Task<StreamReader> GetAsync()
    {
        return Task.FromResult(new StreamReader(new TemplatedStream(stream, placeholderResolver)));
    }
}
