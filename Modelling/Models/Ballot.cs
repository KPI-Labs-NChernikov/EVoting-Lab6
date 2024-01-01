namespace Modelling.Models;
public sealed class Ballot
{
    public byte[] EncryptedCandidateId { get; }
    public byte[] GeneratorSeed { get; }
    public Guid VoterId { get; }

    public Ballot(byte[] encryptedCandidateId, byte[] generatorSeed, Guid voterId)
    {
        EncryptedCandidateId = encryptedCandidateId;
        GeneratorSeed = generatorSeed;
        VoterId = voterId;
    }
}
