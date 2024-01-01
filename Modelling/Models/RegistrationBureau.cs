using Algorithms;
using Algorithms.Abstractions;
using FluentResults;

namespace Modelling.Models;
public sealed class RegistrationBureau
{
    private readonly Dictionary<Guid, Token?> _votersTokens = [];
    private readonly Dictionary<Guid, VoterData?> _voterData = [];
    private readonly Dictionary<string, string> _votersAccounts = [];
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRngProvider _rngProvider;

    public RegistrationBureau(int potentialVotersCount, IPasswordHasher passwordHasher, IRngProvider rngProvider)
    {
        _passwordHasher = passwordHasher;
        _rngProvider = rngProvider;

        for (var i = 0; i < potentialVotersCount; i++)
        {
            var id = Guid.NewGuid();
            _votersTokens.Add(id, null);
            _voterData.Add(id, null);
        }
    }

    public IReadOnlyList<Guid> GetVotersIds => _votersTokens.Keys.ToList();

    public void ReceiveTokens(IReadOnlyList<Token> tokens)
    {
        var intersectionLength = tokens.Select(t => t.VoterId).Intersect(_votersTokens.Keys).Count();
        if (intersectionLength != _votersTokens.Count || intersectionLength != tokens.Count)
        {
            throw new InvalidOperationException("Some voters ids are different between RB and EC.");
        }

        foreach (var token in tokens)
        {
            _votersTokens[token.VoterId] = token;
        }
    }

    public Result<ECProgramUserData> GrantToken(VoterData voter)
    {
        if (_voterData.Values.Any(v => v == voter))
        {
            return Result.Fail("You have already received a token.");
        }

        var voterAbilityResult = voter.IsAbleToVote();
        if (voterAbilityResult.IsFailed)
        {
            return voterAbilityResult;
        }

        var voterId = _voterData.First(v => v.Value is null).Key;

        var token = _votersTokens[voterId];

        _voterData[voterId] = voter;

        var login = $"{voter.FullName.ToLower()}-{voter.BirthDay.ToShortDateString()}";
        var password = PublicConstants.Encoding.GetString(_rngProvider.GenerateNext(8));

        _votersAccounts[login] = _passwordHasher.Hash(password);

        return Result.Ok(new ECProgramUserData(login, password, token!));
    }

    public bool VerifyAccount(string login, string password)
    {
        var accountWasFound = _votersAccounts.TryGetValue(login, out var hashPassword);
        if (!accountWasFound)
        {
            return false;
        }

        return _passwordHasher.Hash(password) == hashPassword;
    }
}
