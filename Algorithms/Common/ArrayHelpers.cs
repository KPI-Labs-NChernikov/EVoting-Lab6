namespace Algorithms.Common;
public static class ArrayHelpers
{
    public static byte[] NormalizeArray(byte[] array, int expectedLength)
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
