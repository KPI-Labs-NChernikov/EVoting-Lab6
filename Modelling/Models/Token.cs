using Algorithms.Common;
using Org.BouncyCastle.Crypto;

namespace Modelling.Models;
public sealed class Token
{
    public Guid SerialNumber { get; }
    public Guid VoterId { get; }
    public BlumBlumShubKey GeneratorPublicKey { get; }

    public Token(Guid serialNumber, Guid voterId, BlumBlumShubKey generatorPublicKey)
    {
        SerialNumber = serialNumber;
        VoterId = voterId;
        GeneratorPublicKey = generatorPublicKey;
    }
}
