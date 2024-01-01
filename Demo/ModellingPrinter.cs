using Modelling.Extensions;
using Modelling.Models;

namespace Demo;
public sealed class ModellingPrinter
{
    private readonly DemoDataFactory _dataFactory;

    public ModellingPrinter(DemoDataFactory dataFactory)
    {
        _dataFactory = dataFactory;
    }

    public void PrintUsualVoting(IReadOnlyList<Candidate> candidates, IReadOnlyList<VoterData> votersData)
    {
        Console.WriteLine("Usual voting:");

        var votersCount = 8;
        var registrationBureau = _dataFactory.CreateRegistrationBureau(votersCount);
        var electionCommission = _dataFactory.CreateElectionCommission(candidates);
        var tokens = electionCommission.GenerateTokens(registrationBureau.VotersIds);
        registrationBureau.ReceiveTokens(tokens);

        var programUserDataList = new List<ECProgramUserData>();
        foreach (var voterData in votersData)
        {
            var result = registrationBureau.GrantToken(voterData);
            if (result.IsSuccess)
            {
                Console.WriteLine($"Token with voter id {result.Value.Token.VoterId} has been granted to user {voterData.FullName}");
                programUserDataList.Add(result.Value);
            }
            else
            {
                result.PrintErrorIfFailed();
            }
        }
        Console.WriteLine();

        var program = _dataFactory.CreateEcProgram(registrationBureau, electionCommission);
        var votersWithCandidateIds = _dataFactory.CreateVotersWithCandidatesIds(
            programUserDataList.Select(d => d.Token.VoterId).ToList(), candidates);
        foreach (var data in programUserDataList)
        {
            var loginResult = program.Login(data.Login, data.Password);

            if (loginResult.IsSuccess)
            {
                Console.WriteLine($"Voter {data.Token.VoterId} logged in successfully");
            }
            else
            {
                loginResult.PrintErrorIfFailed();
            }

            var tokenConnectionResult = program.ConnectToken(data.Token);
            if (tokenConnectionResult.IsSuccess)
            {
                Console.WriteLine($"Voter {data.Token.VoterId} connected the token successfully");
            }
            else
            {
                loginResult.PrintErrorIfFailed();
            }

            var votingResult = program.Vote(votersWithCandidateIds[data.Token.VoterId]);
            if (votingResult.IsSuccess)
            {
                Console.WriteLine($"Voter {data.Token.VoterId} casted their vote");
            }
            else
            {
                loginResult.PrintErrorIfFailed();
            }

            program.Logout();
        }
        Console.WriteLine();

        var results = electionCommission.CalculateResults();
        PrintVotingResults(results);

        Console.WriteLine();
    }

    public void PrintVotingResults(VotingResults results)
    {
        Console.WriteLine("Results:");

        foreach (var candidate in results.CandidatesResults.Values.OrderByVotes())
        {
            Console.WriteLine($"{candidate.Candidate.FullName} (id: {candidate.Candidate.Id}): {candidate.Votes} votes");
        }
    }
}
