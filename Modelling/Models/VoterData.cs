using FluentResults;

namespace Modelling.Models;
public sealed class VoterData : IEquatable<VoterData>
{
    public string FullName { get; }
    public DateOnly BirthDay { get; }

    public VoterData(string fullName, DateOnly birthDay)
    {
        FullName = fullName;
        BirthDay = birthDay;
    }

    public Result IsAbleToVote()
    {
        var currentTime = DateTime.Now;
        if (currentTime.Year - BirthDay.Year < 18 
            || (currentTime.Year - BirthDay.Year == 18 && currentTime.Month - BirthDay.Month < 0)
            || (currentTime.Year - BirthDay.Year == 18 && currentTime.Month - BirthDay.Month == 0 && currentTime.Day - BirthDay.Day < 0))
        {
            return Result.Fail($"Voter {FullName} must be 18 years old.");
        }

        return Result.Ok();
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VoterData);
    }

    public bool Equals(VoterData? other)
    {
        if (other is null)
        {
            return false;
        }

        return BirthDay == other.BirthDay && FullName == other.FullName;
    }

    public static bool operator ==(VoterData? obj1, VoterData? obj2)
    {
        if (obj1 is null && obj2 is null)
        {
            return true;
        }
        if (obj1 is null)
        {
            return false;
        }

        return obj1.Equals(obj2);
    }

    public static bool operator !=(VoterData? obj1, VoterData? obj2) => !(obj1 == obj2);

    public override int GetHashCode()
    {
        return HashCode.Combine(BirthDay, FullName);
    }
}
