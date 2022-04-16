using System;
using System.Diagnostics.CodeAnalysis;

namespace Bits;

public readonly struct Bit
{
    private readonly bool _value;
    private readonly int _iValue;

    private Bit(bool value)
    {
        _value = value;
        _iValue = value ? 1 : 0;
    }

    #region Operators
    
    #region Implicit
    public static implicit operator bool(Bit value) => value._value;
    public static implicit operator byte(Bit value) => value._value ? (byte)1 : (byte)0;
    public static implicit operator sbyte(Bit value) => value._value ? (sbyte)1 : (sbyte)0;
    public static implicit operator short(Bit value) => value._value ? (short)1 : (short)0;
    public static implicit operator ushort(Bit value) => value._value ? (ushort)1 : (ushort)0;
    public static implicit operator int(Bit value) => value._value ? 1 : 0;
    public static implicit operator uint(Bit value) => value._value ? 1U : 0;
    public static implicit operator long(Bit value) => value._value ? 1L : 0;
    public static implicit operator ulong(Bit value) => value._value ? 1UL : 0;
    public static implicit operator Bit(bool value) => new(value);
    public static implicit operator Bit(int value)
    {
        if (value == 0)
            return false;
        else if (value == 1)
            return true;
        else
        {
            throw new ArgumentException("Value must be 0 or 1.");
        }
    }
    #endregion

    public static Bit operator &(Bit left, Bit right) => left._value & right._value;
    public static Bit operator |(Bit left, Bit right) => left._value | right._value;
    public static Bit operator ^(Bit left, Bit right) => left._value ^ right._value;
    public static Bit operator ~(Bit value) => !value._value;
    public static bool operator ==(Bit left, Bit right) => left._value == right._value;
    public static bool operator !=(Bit left, Bit right) => left._value != right._value;

    #endregion

    public override bool Equals([NotNullWhen(true)] object obj) => obj is Bit bit && _value == bit._value;

    public override int GetHashCode() => _value.GetHashCode();

    public override string ToString()
    {
        return _iValue.ToString();
    }
}
