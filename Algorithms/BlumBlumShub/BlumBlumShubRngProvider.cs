using Algorithms.Abstractions;
using Algorithms.Common;
using Org.BouncyCastle.Math;

namespace Algorithms.BlumBlumShub;
public sealed class BlumBlumShubRngProvider : IRngProvider
{
    private BigInteger _x;
    private readonly BigInteger _n;
    public BlumBlumShubRngProvider(byte[] seed, byte[] n)
    {
        _x = new BigInteger(1, seed, 0, seed.Length);
        _n = new BigInteger(1, n, 0, n.Length);
    }

    public byte[] GenerateNext(int length)
    {
        var result = BigInteger.Zero;
        for (var i = 0; i < length * 8; i++)
        {
            _x = _x.ModPow(BigInteger.Two, _n);
            result = result.ShiftLeft(1);
            result = result.Add(_x.Mod(BigInteger.Two));
        }

        return ArrayHelpers.NormalizeArray(result.ToByteArrayUnsigned(), length);
    }
}
