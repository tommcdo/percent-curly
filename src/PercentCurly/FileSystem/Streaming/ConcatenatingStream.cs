using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PercentCurly.FileSystem.Streaming;

/// <summary>
/// A stream that concatenates multiple streams.
/// </summary>
public class ConcatenatingStream : Stream
{
    private readonly IList<Stream> streams;
    private readonly IEnumerator<Stream> enumerator;

    public ConcatenatingStream(IList<Stream> streams)
    {
        if (streams.Count == 0)
        {
            throw new ArgumentException("Must provide at least one stream.", nameof(streams));
        }
        this.streams = streams;
        this.enumerator = streams.GetEnumerator();
        this.enumerator.MoveNext();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => streams.Sum(s => s.Length);

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (enumerator.Current == null)
        {
            return 0;
        }

        // Read from the current stream. If it returns fewer than count bytes, move to the next stream and read from that.
        var bytesRead = enumerator.Current.Read(buffer, offset, count);
        while (bytesRead < count)
        {
            if (!enumerator.MoveNext())
            {
                break;
            }
            bytesRead += enumerator.Current.Read(buffer, offset + bytesRead, count - bytesRead);
        }
        return bytesRead;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}
