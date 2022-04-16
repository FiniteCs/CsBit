using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Bits;

public static class Program
{
    public static void Main()
    {
        var left = new BitArray<int>(42);
        var right = new BitArray<int>(24);

        WriteLine(left);
        WriteLine(right);
        WriteLine(left & right);
        WriteLine(left | right);
        WriteLine(left ^ right);
        WriteLine(~left);
        WriteLine(~right);
        WriteLine(left << 8);
        WriteLine(left >> 8);
        WriteLine(right << 8);
        WriteLine(right >> 8);
        WriteLine(left == right);
        WriteLine(left != right);
        WriteLine(left.Length);
        WriteLine(right.Length);
        WriteLine(left.Reverse());
        WriteLine(right.Reverse());
        WriteLine(left.BitFormatType.Name);
        WriteLine(right.BitFormatType.Name);
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
