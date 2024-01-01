namespace Algorithms.Abstractions;
public interface IRngProvider
{
    byte[] GenerateNext(int length);
}
