using Algorithms.Common;
using Org.BouncyCastle.Crypto;

namespace Modelling.Models;
public sealed class Token
{
    public Guid VoterId { get; }
    public BlumBlumShubKey GeneratorPublicKey { get; }
    public AsymmetricKeyParameter MessageEncryptionPublicKey { get; }

    public Token(Guid voterId, BlumBlumShubKey generatorPublicKey, AsymmetricKeyParameter messageEncryptionPublicKey)
    {
        VoterId = voterId;
        GeneratorPublicKey = generatorPublicKey;
        MessageEncryptionPublicKey = messageEncryptionPublicKey;
    }
}
