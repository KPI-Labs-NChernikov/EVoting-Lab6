using Algorithms.Abstractions;
using Algorithms.Common;
using FluentResults;
using Org.BouncyCastle.Crypto;

namespace Modelling.Models;
public sealed class ElectionCommission
{
    public readonly AsymmetricKeyParameter MessageEncryptionPublicKey;
    private readonly AsymmetricKeyParameter _messageEncryptionPrivateKey;

    private readonly Dictionary<Guid, BlumBlumShubKey> _votersKeys = [];

    private readonly IKeyGenerator<BlumBlumShubKey> _rngKeyGenerator;

    private readonly IEncryptionProvider<AsymmetricKeyParameter> _messageEncryptionProvider;
    private readonly IEncryptionProvider<BlumBlumShubKey> _rngEncryptionProvider;

    private readonly IObjectToByteArrayTransformer _transformer;

    private readonly Dictionary<Guid, int> _votingResults = [];
    private readonly List<Candidate> _candidates;

    public ElectionCommission(IEnumerable<Candidate> candidates, Keys<AsymmetricKeyParameter> messageEncryptionKeys, IKeyGenerator<BlumBlumShubKey> rngKeyGenerator, IEncryptionProvider<AsymmetricKeyParameter> messageEncryptionProvider, IEncryptionProvider<BlumBlumShubKey> rngEncryptionProvider, IObjectToByteArrayTransformer transformer)
    {
        _candidates = candidates.ToList();

        MessageEncryptionPublicKey = messageEncryptionKeys.PublicKey;
        _messageEncryptionPrivateKey = messageEncryptionKeys.PrivateKey;

        _rngKeyGenerator = rngKeyGenerator;

        _messageEncryptionProvider = messageEncryptionProvider;
        _rngEncryptionProvider = rngEncryptionProvider;

        _transformer = transformer;
    }

    public IReadOnlyList<Token> GenerateTokens(IReadOnlyList<Guid> votersIds)
    {
        var tokens = new List<Token>();
        foreach (var voterId in votersIds)
        {
            var keys = _rngKeyGenerator.Generate();
            _votersKeys[voterId] = keys.PrivateKey;
            tokens.Add(new Token(Guid.NewGuid(), voterId, keys.PublicKey));
        }
        return tokens;
    }

    public Result AcceptBallot(byte[] encryptedBallot)
    {
        return Result.Try(
                () => _transformer.ReverseTransform<Ballot>(
                          _messageEncryptionProvider.Decrypt(encryptedBallot, _messageEncryptionPrivateKey))
                      ?? throw new InvalidOperationException("Value cannot be transformed to a ballot."),
                e => new Error("Message has wrong format or was incorrectly encrypted.").CausedBy(e))
            .Bind(b => _votingResults.ContainsKey(b.VoterId)
                ? Result.Fail("Voter has already casted a vote.")
                : Result.Ok(b))
            .Bind(DecryptCandidateId)
            .Bind(VerifyCandidateId)
            .Bind(AddVote);
    }

    private Result<(Guid, int)> DecryptCandidateId(Ballot ballot)
    {
        var privateKeyExists = _votersKeys.TryGetValue(ballot.VoterId, out var key);
        if (!privateKeyExists)
        {
            return Result.Fail("User is not registered in EC.");
        }

        key = key.WithX0(ballot.GeneratorSeed);

        return Result.Try(
            () => _transformer.ReverseTransform<int>(
                      _rngEncryptionProvider.Decrypt(ballot.EncryptedCandidateId, key)),
            e => new Error("Message has wrong format or was incorrectly encrypted.").CausedBy(e))
            .Bind(c => Result.Ok((ballot.VoterId, c)));
    }

    private Result<(Guid, int)> VerifyCandidateId((Guid, int) ids)
    {
        if (_candidates.All( c => c.Id != ids.Item2))
        {
            return Result.Fail("The candidate was not found");
        }

        return Result.Ok(ids);
    }

    private Result AddVote((Guid, int) ids)
    {
        _votingResults[ids.Item1] = ids.Item2;
        return Result.Ok();
    }

    public VotingResults CalculateResults()
    {
        var results = new VotingResults();
        foreach (var candidate in _candidates)
        {
            results.CandidatesResults.Add(candidate.Id, new(candidate));
        }

        foreach (var result in _votingResults)
        {
            results.CandidatesResults[result.Value].Votes++;
        }

        return results;
    }
}
