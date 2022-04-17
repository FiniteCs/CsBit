using System.Collections.Generic;
using System.Linq;

namespace Bits;

public static class Extensions
{
    // Will be used when I start working on bit shifting
    public static T[] Subsequence<T>(this IEnumerable<T> arr, int startIndex, int length)
    {
        return arr.Skip(startIndex).Take(length).ToArray();
    }
}
