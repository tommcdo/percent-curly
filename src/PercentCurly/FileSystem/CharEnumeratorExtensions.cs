using System;

namespace PercentCurly.FileSystem;

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
