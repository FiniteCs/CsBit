using System;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace Bits;

public static class Program
{
    public static void Main()
    {
        var bitList = new BitArray<int>(10342);
        var bitListCopy = new BitArray<int>(bitList);
        Console.WriteLine(bitList == bitListCopy);

        Console.WriteLine(bitList.ToString(true));
        Console.WriteLine(bitList.ReadBits());;

        Console.WriteLine();

        var bitList2 = new BitArray<Point>(new Point(2342, 5457));
        Console.WriteLine(bitList2.ToString(true));
        Console.WriteLine(bitList2.ReadBits());

        var bitList4 = bitList << 10;
        Console.WriteLine(bitList4.ToString(true));
    }
}
