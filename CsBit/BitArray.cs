using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bits;

public readonly struct BitArray<T> : 
    IEquatable<BitArray<T>>,
    IEnumerable<Bit>
    where T : unmanaged
{
    private readonly int _size;
    private readonly T _value;
    private readonly Bit[] _bits;
    private readonly IList<Bit> _list;

    public unsafe BitArray(T value)
    {
        _size = sizeof(T) * 8;
        _value = value;
        _bits = GetBits(_value);
        _list = new List<Bit>(_bits);
    }

    public Bit this[int index] => _bits[index];

    private static unsafe Bit[] GetBits(byte value)
    {
        var count = sizeof(byte) * 8;
        var bytes = new int[count];
        for (var i = 0; value > 0; i++)
        {
            bytes[i] = value % 2;
            value /= 2;
        }

        Array.Reverse(bytes);
        var bits = new Bit[bytes.Length];
        for (var i = 0; i < bytes.Length; i++)
        {
            bits[i] = bytes[i];
        }

        return bits;
    }

    private static unsafe Bit[] GetBits(T value)
    {
        var size = sizeof(T);
        var bytes = new byte[size];

        nint ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(value, ptr, false);
        Marshal.Copy(ptr, bytes, 0, size);
        Marshal.FreeHGlobal(ptr);
        var bits = new List<Bit>();
        for (var i = 0; i < size; i++)
        {
            var byteBits = GetBits(bytes[i]);
            bits.AddRange(byteBits);
        }

        return bits.ToArray();
    }

    public byte[] GetBytes()
    {
        var bits = _bits;
        var count = bits.Length / 8;
        Bit[][] bitmap = new Bit[count][];
        var position = 0;
        for (int i = 0; i < count; i++)
        {
            var array = new Bit[8];
            var index = position;
            for (int j = 0; j < array.Length; j++)
            {
                array[j] = bits[index];
                index++;
            }

            position += array.Length;
            bitmap[i] = array;
        }

        var bytes = new byte[bitmap.Length];
        for (int i = 0; i < bitmap.Length; i++)
        {
            var bitArray = bitmap[i];
            byte b = 0;
            var k = 0;
            for (int j = bitArray.Length - 1; j >= 0; j--)
            {
                int value = bitArray[k];
                var bitValue = value * Math.Pow(2, j);
                b += (byte)bitValue;
                k++;
            }

            bytes[i] = b;
        }

        return bytes;
    }

    public T ReadBits()
    {
        var bytes = GetBytes();
        return Unsafe.ReadUnaligned<T>(ref bytes[0]);
    }

    public bool Equals(BitArray<T> other)
    {
        return GetHashCode() == other.GetHashCode();
    }

    public IEnumerator<Bit> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public override int GetHashCode()
    {
        var hashList = new List<int>();
        foreach (var bit in this)
        {
            hashList.Add(bit.GetHashCode());
        }

        hashList.Add(_size);
        var result = 0;
        for (var i = 0; i < hashList.Count; i++)
        {
            result ^= hashList[i];
        }

        return result;
    }

    public override bool Equals(object obj)
    {
        return obj is BitArray<T> array && Equals(array);
    }

    public static bool operator ==(BitArray<T> left, BitArray<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BitArray<T> left, BitArray<T> right)
    {
        return !(left == right);
    }

    public override string ToString()
    {
        var str = string.Join("", this);
        return str;
    }
}
