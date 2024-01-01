using System.Text;

namespace Algorithms;
public static class PublicConstants
{
    public const int IntSize = sizeof(int);
    public const int GuidSize = 16;

    public static readonly Encoding Encoding = Encoding.UTF8;

    public const int ElGamalKeySize = 1024;

    public const int BBSPQBitsCount = 256;
    public const int BBSNX0BitsCount = BBSPQBitsCount * 2;
}
