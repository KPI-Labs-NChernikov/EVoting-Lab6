using Algorithms.Abstractions;
using Modelling.Models;
using Org.BouncyCastle.Crypto;
using Algorithms.Common;

namespace Demo;
public sealed class DemoDataFactory
{
    private readonly IEncryptionProvider<AsymmetricKeyParameter> _messageEncryptionProvider;
    private readonly IKeyGenerator<AsymmetricKeyParameter> _messageEncryptionKeyGenerator;

    private readonly IEncryptionProvider<BlumBlumShubKey> _rngEncryptionProvider;
    private readonly IKeyGenerator<BlumBlumShubKey> _rngEncryptionKeyGenerator;
    private readonly ISeedGenerator _seedGenerator;
    private readonly IRngProvider _rngProvider;

    private readonly IObjectToByteArrayTransformer _transformer;
    private readonly IPasswordHasher _passwordHasher;

    public DemoDataFactory(IEncryptionProvider<AsymmetricKeyParameter> messageEncryptionProvider, IKeyGenerator<AsymmetricKeyParameter> messageEncryptionKeyGenerator, IEncryptionProvider<BlumBlumShubKey> rngEncryptionProvider, IKeyGenerator<BlumBlumShubKey> rngEncryptionKeyGenerator, ISeedGenerator seedGenerator, IRngProvider rngProvider, IObjectToByteArrayTransformer transformer, IPasswordHasher passwordHasher)
    {
        _messageEncryptionProvider = messageEncryptionProvider;
        _messageEncryptionKeyGenerator = messageEncryptionKeyGenerator;
        _rngEncryptionProvider = rngEncryptionProvider;
        _rngEncryptionKeyGenerator = rngEncryptionKeyGenerator;
        _seedGenerator = seedGenerator;
        _rngProvider = rngProvider;
        _transformer = transformer;
        _passwordHasher = passwordHasher;
    }

    public IReadOnlyList<Candidate> CreateCandidates()
    {
        return new List<Candidate>
        {
            new (1, "Ishaan Allison"),
            new (2, "Oliver Mendez"),
            new (3, "Naomi Winter"),
        };
    }

    public IReadOnlyList<VoterData> CreateVotersData()
    {
        return new List<VoterData>
        {
            new ("Jasper Lambert", new DateOnly(2000, 11, 15)),
            new ("Jonty Levine", new DateOnly(2010, 1, 2)),      // Not capable.
            new ("Nathaniel Middleton", new DateOnly(1981, 1, 2)),
            new ("Nathan Bass", new DateOnly(1995, 1, 25)),
            new ("Aran Doyle", new DateOnly(2003, 1, 7)),
            new ("Julian Harper", new DateOnly(1937, 9, 2)),
            new ("Lucian Gross", new DateOnly(1999, 12, 20)),
            new ("Alicia Sierra", new DateOnly(1981, 1, 2))
        };
    }

    public RegistrationBureau CreateRegistrationBureau(int potentialVotersCount)
    {
        return new RegistrationBureau(potentialVotersCount, _passwordHasher, _rngProvider);
    }

    public ElectionCommission CreateElectionCommission(IReadOnlyList<Candidate> candidates)
    {
        return new ElectionCommission(candidates, _messageEncryptionKeyGenerator.Generate(), _rngEncryptionKeyGenerator, _messageEncryptionProvider, _rngEncryptionProvider, _transformer);
    }

    public ECProgram CreateEcProgram(RegistrationBureau registrationBureau, ElectionCommission electionCommission)
    {
        return new ECProgram(_seedGenerator, _messageEncryptionProvider, _rngEncryptionProvider, _transformer,
            electionCommission, registrationBureau);
    }

    public Dictionary<Guid, int> CreateVotersWithCandidatesIds(IReadOnlyList<Guid> voters, IReadOnlyList<Candidate> candidates)
    {
        var dictionary = new Dictionary<Guid, int>();
        for (var i = 0; i < voters.Count; i++)
        {
            var candidateIndex = (i % 8 + 1) switch
            {
                1 => 0,
                2 => 0,

                3 => 1,
                4 => 1,
                5 => 2,
                6 => 2,
                7 => 2,
                8 => 0,     // Unable goes here.

                _ => throw new InvalidOperationException("Negative and zero voters' ids are not supported in this method.")
            };
            dictionary.Add(voters[i], candidates[candidateIndex].Id);
        }
        return dictionary;
    }
}
