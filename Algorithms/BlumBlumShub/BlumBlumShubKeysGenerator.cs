using Algorithms.Abstractions;
using Algorithms.Common;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace Algorithms.BlumBlumShub;
public sealed class BlumBlumShubKeysGenerator : IKeyGenerator<BlumBlumShubKey>, ISeedGenerator
{
    private readonly Random _random = new Random();

    public Keys<BlumBlumShubKey> Generate()
    {
        var byteLength = IntHelpers.BitsCountToBytesCount(PublicConstants.BBSPQBitsCount);
        var p = GeneratePrimeCongruent3Mod4(byteLength);
        var q = GeneratePrimeCongruent3Mod4(byteLength);
        var n = p.Multiply(q);
        var nAsByteArray = NormalizeArray(n.ToByteArrayUnsigned(), byteLength * 2);
        return new Keys<BlumBlumShubKey>(
            new BlumBlumShubKey
            {
                N = nAsByteArray,
            },
            new BlumBlumShubKey
            {
                N = nAsByteArray,
                P = p.ToByteArrayUnsigned(),
                Q = q.ToByteArrayUnsigned()
            });
    }

    public byte[] GenerateSeed(byte[] n)
    {
        var x0ByteLength = IntHelpers.BitsCountToBytesCount(PublicConstants.BBSNX0BitsCount);
        var nInteger = new BigInteger(1, n, 0, n.Length);
        var x0 = GenerateX(nInteger).ModPow(BigInteger.Two, nInteger);
        return NormalizeArray(x0.ToByteArrayUnsigned(), x0ByteLength);
    }

    private BigInteger GeneratePrimeCongruent3Mod4(int byteLength)
    {
        BigInteger number;
        do
        {
            var numberAsArray = new byte[byteLength];
            _random.NextBytes(numberAsArray);
            number = new BigInteger(1, numberAsArray, 0, numberAsArray.Length);
        } while (!number.Mod(BigInteger.Four).Equals(BigInteger.Three) || !number.IsProbablePrime(100));

        return number;
    }

    private BigInteger GenerateX(BigInteger n)
    {
        BigInteger x;
        do
        {
            x = new BigInteger(PublicConstants.BBSPQBitsCount, new SecureRandom()).Mod(n);
        } while (x.CompareTo(BigInteger.One) <= 0
                 || !x.Gcd(n).Equals(BigInteger.One));

        return x;
    }

    private static byte[] NormalizeArray(byte[] array, int expectedLength)
    {
        if (expectedLength == array.Length)
        {
            return array;
        }

        var result = new byte[expectedLength];
        var difference = expectedLength - array.Length;
        Buffer.BlockCopy(array, 0, result, difference, array.Length);
        return result;
    }
}
