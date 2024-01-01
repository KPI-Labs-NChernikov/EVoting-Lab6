using Algorithms.Abstractions;
using Algorithms.Common;
using Org.BouncyCastle.Math;

namespace Algorithms.BlumBlumShub;

public sealed class BBSXorEncryptionProvider : IEncryptionProvider<BlumBlumShubKey>
{
    public byte[] Encrypt(byte[] message, BlumBlumShubKey key)
    {
        if (key.X0 is null)
        {
            throw new ArgumentNullException(nameof(key), "X0 cannot be null. Generate is separately.");
        }

        var x = new BigInteger(1, key.X0, 0, key.X0.Length);
        var n = new BigInteger(1, key.N, 0, key.N.Length);

        var sequence = Generate(x, n, message.Length);

        return Xor(message, sequence);
    }

    public byte[] Decrypt(byte[] message, BlumBlumShubKey key)
    {
        if (key.X0 is null)
        {
            throw new ArgumentNullException(nameof(key), "X0 cannot be null.");
        }

        var x = new BigInteger(1, key.X0, 0, key.X0.Length);
        var n = new BigInteger(1, key.P, 0, key.P.Length).Multiply(new BigInteger(1, key.Q, 0, key.Q.Length));

        var sequence = Generate(x, n, message.Length);

        return Xor(message, sequence);
    }

    private static byte[] Generate(BigInteger x, BigInteger n, int length)
    {
        var result = BigInteger.Zero;
        for (var i = 0; i < length * 8; i++)
        {
            x = x.ModPow(BigInteger.Two, n);
            result = result.ShiftLeft(1);
            result = result.Add(x.Mod(BigInteger.Two));
        }

        return ArrayHelpers.NormalizeArray(result.ToByteArrayUnsigned(), length);
    }

    private static byte[] Xor(byte[] data, byte[] key)
    {
        var encrypted = new byte[data.Length];

        for (var i = 0; i < encrypted.Length; i++)
        {
            encrypted[i] = (byte)(data[i] ^ key[i % key.Length]);
        }

        return encrypted;
    }
}
