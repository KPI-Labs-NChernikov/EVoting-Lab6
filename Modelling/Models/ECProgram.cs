using Algorithms.Abstractions;
using Algorithms.Common;
using FluentResults;
using Org.BouncyCastle.Crypto;

namespace Modelling.Models;
public sealed class ECProgram
{
    private readonly ISeedGenerator _seedGenerator;

    private readonly IEncryptionProvider<AsymmetricKeyParameter> _messageEncryptionProvider;
    private readonly IEncryptionProvider<BlumBlumShubKey> _rngEncryptionProvider;
    private readonly IObjectToByteArrayTransformer _transformer;

    private readonly ElectionCommission _electionCommission;
    private readonly RegistrationBureau _registrationBureau;

    private bool _userIsLoggedIn;
    private Token? _currentToken;

    public ECProgram(ISeedGenerator seedGenerator, IEncryptionProvider<AsymmetricKeyParameter> messageEncryptionProvider, IEncryptionProvider<BlumBlumShubKey> rngEncryptionProvider, IObjectToByteArrayTransformer transformer, ElectionCommission electionCommission, RegistrationBureau registrationBureau)
    {
        _seedGenerator = seedGenerator;
        _messageEncryptionProvider = messageEncryptionProvider;
        _rngEncryptionProvider = rngEncryptionProvider;
        _transformer = transformer;
        _electionCommission = electionCommission;
        _registrationBureau = registrationBureau;
    }

    public Result Login(string login, string password)
    {
        var result = _registrationBureau.VerifyAccount(login, password);

        _userIsLoggedIn = true;

        return Result.FailIf(result, new Error("Log in failed"));
    }

    public Result ConnectToken(Token token)
    {
        if (!_userIsLoggedIn)
        {
            return Result.Fail(new Error("Log in first"));
        }

        _currentToken = token;
        return Result.Ok();
    }

    public Result Vote(int candidateId)
    {
        if (_currentToken is null)
        {
            return Result.Fail("Connect the token first");
        }

        var x0 = _seedGenerator.GenerateSeed(_currentToken.GeneratorPublicKey.N);
        var key = _currentToken.GeneratorPublicKey.WithX0(x0);
        var encryptedCandidateId = _rngEncryptionProvider.Encrypt(_transformer.Transform(candidateId), key);
        var ballot = new Ballot(encryptedCandidateId, x0, _currentToken.VoterId);
        var encryptedBallot = _messageEncryptionProvider.Encrypt(_transformer.Transform(ballot),
            _electionCommission.MessageEncryptionPublicKey);

        return _electionCommission.AcceptBallot(encryptedBallot);
    }

    public void Logout()
    {
        _userIsLoggedIn = false;
        _currentToken = null;
    }
}
