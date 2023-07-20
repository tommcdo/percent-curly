using System;
using System.Collections.Generic;

namespace PercentCurly.FileSystem;

public abstract class ByteStream : ReadOnlyStream
{
    private readonly Lazy<IEnumerator<byte>> _bytes;

    public ByteStream()
    {
        _bytes = new Lazy<IEnumerator<byte>>(() => Bytes.GetEnumerator());
    }

    private IEnumerator<byte> BytesEnumerator => _bytes.Value;

    protected abstract IEnumerable<byte> Bytes { get; }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int i = 0;
        for (; i < count && BytesEnumerator.MoveNext(); i++)
        {
            buffer[offset + i] = BytesEnumerator.Current;
        }
        return i;
    }
}
