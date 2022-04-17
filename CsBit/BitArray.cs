using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Bits;

public readonly struct BitArray<T>: 
    ICollection,
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

    #region Properties
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

    public Type BitFormatType => typeof(T);

    public int Count => _size;

    public bool IsSynchronized => false;

    public object SyncRoot => null;

    public bool IsReadOnly => throw new NotImplementedException();
    #endregion

    #region Methods
    private static TType[] ShiftArray<TType>(TType[] source, TType[] items, int shift)
    {
        var poppedItems = new TType[shift];
        for (int j = 0; j < shift; j++)
        {
            poppedItems[j] = source[j];
        }

        var temp = new TType[source.Length - shift];
        Array.Copy(source, shift, temp, 0, temp.Length);

        var shiftedArray = new TType[temp.Length + items.Length];
        Array.Copy(temp, 0, shiftedArray, 0, temp.Length);
        Array.Copy(items, 0, shiftedArray, temp.Length, items.Length);
        return shiftedArray;
    }

    public BitArray<T> Reverse()
    {
        var bitArray = new Bit[Count];
        for (int i = 0; i < Count; i++)
        {
            bitArray[i] = this[Count - i - 1];
        }

        return new BitArray<T>(bitArray);
    }
    
    public Bitmap GetSegments()
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

        return new Bitmap(bitmap);
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

    public void CopyTo(Array array, int index)
    {
        Array.Copy(array, _bits, index);
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
        var sb = new StringBuilder();
        var segments = GetSegments();
        for (var i = 0; i < segments.Length; i++)
        {
            BitArray<T> segment = new BitArray<T>(segments[i].ToArray());
            var str = string.Join("", segment);
            sb.Append(str + " ");
        }

        return sb.ToString();
    }
    #endregion
}

public readonly struct Bitmap
{
    private readonly Bit[][] _bitmap;
    private readonly int _allBits;
    private readonly int _size;
        
    public Bitmap(Bit[][] bitmap)
    {
        _bitmap = bitmap;
        var allBits = 0;
        for (var i = 0; i < bitmap.Length; i++)
        {
            allBits += bitmap[i].Length;
        }

        _allBits = allBits;
        _size = bitmap.Length;
    }
        
    public Bit[] this[int index]
    {
        get => _bitmap[index];
        set => _bitmap[index] = value;
    }

    public int Length => _size;

    public int AllBitsCount => _allBits;

    public Bit[][] ToArray() => _bitmap;

    public BitArray<T> ToBitArray<T>()
        where T : unmanaged
    {
        var bitArray = new Bit[_allBits];
        var pos = 0;
        for (int i = 0; i < _size; i++)
        {
            var arr = this[i];
            for (int j = 0; j < arr.Length; j++)
            {
                bitArray[pos] = arr[j];
                pos++;
            }
        }

        return new BitArray<T>(bitArray);
    }
}

public readonly struct BitStack:
    IEnumerable<Bit>,
    ICollection
{
    private readonly Stack<Bit> _stack;
    public BitStack()
    {
        _stack = new Stack<Bit>();
    }

    public int Count => _stack.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => null;

    public void Push(Bit bit)
    {
        _stack.Push(bit);
    }

    public Bit Pop()
    {
        return _stack.Pop();
    }

    public void PushRange(Bit[] bits)
    {
        for (int i = bits.Length - 1; i >= 0; i--)
        {
            _stack.Push(bits[i]);
        }
    }

    public Bit[] PopRange(int count)
    {
        var bits = new Bit[count];
        for (int i = 0; i < count; i++)
        {
            bits[i] = _stack.Pop();
        }

        return bits;
    }

    public void CopyTo(Array array, int index)
    {
        Array.Copy(_stack.ToArray(), 0, array, index, _stack.Count);
    }

    public IEnumerator<Bit> GetEnumerator()
    {
        return _stack.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
