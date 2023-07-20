using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PercentCurly.FileSystem;

public class ReadOnlyStream : Stream
{
    private readonly IEnumerator<byte> bytes;

    public ReadOnlyStream(IEnumerable<byte> bytes)
    {
        this.bytes = bytes.GetEnumerator();
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush() => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count)
    {
        int i = 0;
        for (; i < count && bytes.MoveNext(); i++)
        {
            buffer[offset + i] = bytes.Current;
        }
        return i;
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
}

public class TemplatedStream : ReadOnlyStream
{
    public TemplatedStream(Stream stream, IPlaceholderResolver placeholderResolver)
        : base(CharsToBytes(ProcessStream(stream, placeholderResolver)))
    {
    }

    private static IEnumerable<byte> CharsToBytes(IEnumerable<char> chars)
    {
        foreach (var c in chars)
        {
            var bytes = Encoding.UTF8.GetBytes(new[] { c });
            foreach (var b in bytes)
            {
                yield return b;
            }
        }
    }

    private static IEnumerable<char> ProcessStream(Stream stream, IPlaceholderResolver placeholderResolver)
    {
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                continue;
            }
            var chars = line.GetEnumerator();
            while (chars.MoveNext())
            {
                if (chars.Current == '%')
                {
                    if (!chars.MoveNext())
                    {
                        yield return '%';
                        break;
                    }

                    if (chars.Current == '{')
                    {
                        // Build the placeholder name with chars until we hit a '}'.
                        StringBuilder builder = new();
                        while (chars.MoveNextOrThrow(new Exception("Placeholder was not terminated.")) && chars.Current != '}')
                        {
                            builder.Append(chars.Current);
                        }

                        // Resolve the placeholder name and yield the result.
                        var placeholderName = builder.ToString();
                        var placeholderValue = placeholderResolver.Resolve(placeholderName) ?? throw new Exception($"Placeholder '{placeholderName}' could not be resolved.");
                        var placeholderChars = placeholderValue.GetEnumerator();
                        while (placeholderChars.MoveNext())
                        {
                            yield return placeholderChars.Current;
                        }
                    }
                    else
                    {
                        yield return '%';
                        yield return chars.Current;
                    }
                }
                else
                {
                    yield return chars.Current;
                }
            }
            yield return '\n';
        }
    }
}

internal static class CharEnumeratorExtensions
{
    internal static bool MoveNextOrThrow(this CharEnumerator chars, Exception e)
    {
        if (!chars.MoveNext())
        {
            throw e;
        }
        return true;
    }
}
