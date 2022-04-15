using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Bits;

public readonly struct BitArray<T> : 
    IEquatable<BitArray<T>>,
    IEnumerable<Bit>
    where T : unmanaged
{
    private const int BITS_PER_BYTE = 8;
    private readonly int _size;
    private readonly Bit[] _bits;
    private readonly IList<Bit> _list;

    public unsafe BitArray(T value)
    {
        _size = sizeof(T) * BITS_PER_BYTE;
        _bits = GetBits(value);
        _list = new List<Bit>(_bits);
    }

    public unsafe BitArray(Bit[] bits)
    {
        _size = sizeof(T) * BITS_PER_BYTE;
        _bits = bits;
        _list = new List<Bit>(_bits);
    }

    public unsafe BitArray(BitArray<T> other)
    {
        _size = sizeof(T) * BITS_PER_BYTE;
        _bits = other._bits;
        _list = new List<Bit>(_bits);
    }

    public Bit this[int index]
    {
        get
        {
            return _bits[index];
        }

        set
        {
            _bits[index] = value;
        }
    }

    private static unsafe Bit[] GetBits(byte value)
    {
        var count = sizeof(byte) * BITS_PER_BYTE;
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
    
    public static BitArray<T> operator &(BitArray<T> left, BitArray<T> right)
    {
        var bits = new Bit[left._bits.Length];
        for (var i = 0; i < bits.Length; i++)
        {
            bits[i] = left._bits[i] & right._bits[i];
        }

        return new BitArray<T>(bits);
    }

    public static BitArray<T> operator |(BitArray<T> left, BitArray<T> right)
    {
        var bits = new Bit[left._bits.Length];
        for (var i = 0; i < bits.Length; i++)
        {
            bits[i] = left._bits[i] | right._bits[i];
        }

        return new BitArray<T>(bits);
    }

    public static BitArray<T> operator ^(BitArray<T> left, BitArray<T> right)
    {
        var size = Math.Max(left._size, right._size);
        var bits = new Bit[size];
        for (var i = 0; i < size; i++)
        {
            bits[i] = left[i] ^ right[i];
        }

        return new BitArray<T>(bits);
    }

    public static BitArray<T> operator ~(BitArray<T> bits)
    {
        var size = bits._size;
        var result = new Bit[size];
        for (var i = 0; i < size; i++)
        {
            result[i] = ~bits[i];
        }

        return new BitArray<T>(result);
    }

    public static BitArray<T> operator !(BitArray<T> bits)
    {
        var size = bits._size;
        var result = new Bit[size];
        for (var i = 0; i < size; i++)
        {
            result[i] = !bits[i];
        }

        return new BitArray<T>(result);
    }

    public static BitArray<T> operator <<(BitArray<T> bits, int shift)
    {
        var size = bits._size;
        var result = new Bit[size];
        for (var i = 0; i < size; i++)
        {
            result[i] = bits[(i + shift) % size];
        }

        return new BitArray<T>(result);
    }

    public static BitArray<T> operator >>(BitArray<T> bits, int shift)
    {
        var size = bits._size;
        var result = new Bit[size];
        for (var i = 0; i < size; i++)
        {
            result[i] = bits[(i + size - shift) % size];
        }

        return new BitArray<T>(result);
    }

    public static bool operator ==(BitArray<T> left, BitArray<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BitArray<T> left, BitArray<T> right)
    {
        return !(left == right);
    }

    public static implicit operator BitArray<T>(T value) => new(value);

    public static implicit operator BitArray<T>(Bit[] bits) => new(bits);

    public Bit[][] GetSegments()
    {
        var bits = _bits;
        var count = bits.Length / BITS_PER_BYTE;
        Bit[][] bitmap = new Bit[count][];
        var position = 0;
        for (int i = 0; i < count; i++)
        {
            var array = new Bit[BITS_PER_BYTE];
            var index = position;
            for (int j = 0; j < array.Length; j++)
            {
                array[j] = bits[index];
                index++;
            }

            position += array.Length;
            bitmap[i] = array;
        }

        return bitmap;
    }

    public byte[] GetBytes()
    {
        var bitmap = GetSegments();
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

    public override string ToString()
    {
        var str = string.Join("", this);
        return str;
    }

    public string ToString(bool segmented)
    {
        if (segmented)
        {
            var sb = new StringBuilder();
            var segments = GetSegments();
            for (var i = 0; i < segments.Length; i++)
            {
                BitArray<T> segment = new BitArray<T>(segments[i]);
                sb.Append(segment.ToString() + " ");
            }

            return sb.ToString();
        }

        return ToString();
    }
}
