namespace Algorithms.Abstractions;
public interface IPasswordHasher
{
    string Hash(string password);
}
