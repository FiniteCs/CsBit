using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Bits;

public static class Program
{
    public static void Main()
    {
        var bitList = new BitArray<int>(10342);
        Console.WriteLine(bitList);
        Console.WriteLine(bitList.ReadBits());;

        Console.WriteLine();

        var bitList2 = new BitArray<Point>(new Point(2342, 5457));
        Console.WriteLine(bitList2);
        Console.WriteLine(bitList2.ReadBits());
    }
}
