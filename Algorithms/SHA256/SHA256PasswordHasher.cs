using Algorithms.Abstractions;

namespace Algorithms.SHA256;
public sealed class SHA256PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var inputBytes = PublicConstants.Encoding.GetBytes(password);
        var hashedBytes = System.Security.Cryptography.SHA256.HashData(inputBytes);
        return PublicConstants.Encoding.GetString(hashedBytes);
    }
}
