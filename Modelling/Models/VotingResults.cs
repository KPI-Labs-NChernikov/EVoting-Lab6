namespace Modelling.Models;
public sealed class VotingResults
{
    public SortedDictionary<int, CandidateResult> CandidatesResults { get; } = [];
}
