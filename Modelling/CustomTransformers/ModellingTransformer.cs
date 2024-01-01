using Algorithms;
using Algorithms.Abstractions;
using Algorithms.Common;
using Modelling.Models;

namespace Modelling.CustomTransformers;
public sealed class ModellingTransformer : IObjectToByteArrayTransformer
{
    public bool CanTransform(Type type)
    {
        return type == typeof(Ballot)
            || type == typeof(Guid)
            || type == typeof(int);
    }

    public T? ReverseTransform<T>(byte[] data)
    {
        var span = data.AsSpan();
        if (typeof(T) == typeof(Ballot))
        {
            var voterId = ReverseTransform<Guid>(span.Slice(span.Length - PublicConstants.GuidSize).ToArray());
            var generatorSeedLength = IntHelpers.BitsCountToBytesCount(PublicConstants.BBSNX0BitsCount);
            var generatorSeed = span.Slice(
                span.Length - PublicConstants.GuidSize - generatorSeedLength,
                generatorSeedLength).ToArray();
            var encryptedCandidateId = span.Slice(0, span.Length - PublicConstants.GuidSize - generatorSeedLength).ToArray();
            return (T)(object)new Ballot(encryptedCandidateId, generatorSeed, voterId);
        }
        if (typeof(T) == typeof(Guid))
        {
            return (T)(object)new Guid(data);
        }

        if (typeof(T) == typeof(int))
        {
            return (T)(object)BitConverter.ToInt32(span);
        }

        throw new NotSupportedException($"The type {typeof(T)} is not supported.");
    }

    public byte[] Transform(object obj)
    {
        if (obj is Ballot ballot)
        {
            using var stream = new MemoryStream();
            stream.Write(ballot.EncryptedCandidateId);
            stream.Write(ballot.GeneratorSeed);
            stream.Write(Transform(ballot.VoterId));
            return stream.ToArray();
        }
        if (obj is Guid guid)
        {
            return guid.ToByteArray();
        }

        if (obj is int number)
        {
            return BitConverter.GetBytes(number);
        }

        throw new NotSupportedException($"The type {obj.GetType()} is not supported.");
    }
}
