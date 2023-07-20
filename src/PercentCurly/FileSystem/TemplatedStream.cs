using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PercentCurly.FileSystem;

public class TemplatedStream : ByteStream
{
    private readonly Lazy<IEnumerable<byte>> _bytes;

    public TemplatedStream(Stream stream, IPlaceholderResolver placeholderResolver)
    {
        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (placeholderResolver == null)
        {
            throw new ArgumentNullException(nameof(placeholderResolver));
        }

        _bytes = new Lazy<IEnumerable<byte>>(() => CharsToBytes(ProcessStream(stream, placeholderResolver)));
    }

    protected override IEnumerable<byte> Bytes => _bytes.Value;

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
