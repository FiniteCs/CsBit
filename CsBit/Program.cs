#define OPERATIONS
using System;

namespace Bits;

public static class Program
{
    public static void Main()
    {
        var left = new BitArray<int>(42);
        var right = new BitArray<int>(24);

#if OPERATIONS
        #region BitArray operations
        WriteLine(left);
        WriteLine(right);
        WriteLine(left & right);
        WriteLine(42 & 24);
        WriteLine(left | right);
        WriteLine(42 | 24);
        WriteLine(left ^ right);
        WriteLine(42 ^ 24);
        WriteLine(~left);
        WriteLine(~42);
        WriteLine(~right);
        WriteLine(~24);
        WriteLine(left << 8);
        WriteLine(42 << 8);
        WriteLine(left >> 8);
        WriteLine(42 >> 8);
        WriteLine(right << 8);
        WriteLine(24 << 8);
        WriteLine(right >> 8);
        WriteLine(24 >> 8);
        WriteLine(left == right);
        WriteLine(42 == 24);
        WriteLine(left != right);
        WriteLine(42 != 24);
        WriteLine(left.Count);
        WriteLine(right.Count);
        WriteLine(left.BitFormatType.Name);
        WriteLine(right.BitFormatType.Name);
        #endregion
#endif
    }

    public static void WriteLine<T>(BitArray<T> value)
        where T : unmanaged
    {
        Console.WriteLine(value);
        Console.WriteLine(value.ReadBits());
        Console.WriteLine();
    }

    public static void WriteLine(object value)
    {
        Console.WriteLine(value);
        Console.WriteLine();
    }
}
