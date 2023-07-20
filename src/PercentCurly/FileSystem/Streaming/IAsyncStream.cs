using System;
using System.Threading.Tasks;

namespace PercentCurly.FileSystem.Streaming;

public interface IAsyncStream : IAsyncDisposable
{
    Task<byte[]> ReadAsync(int bufferSize);
}
