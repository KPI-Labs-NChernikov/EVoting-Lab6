﻿using Algorithms.Abstractions;

namespace Algorithms.Common;
public sealed class RandomProvider : IRandomProvider
{
    private readonly Random _random = new();

    public T NextItem<T>(IEnumerable<T> items)
    {
        return items.ElementAt(_random.Next(items.Count()));
    }

    public int NextInt(int max) 
    {
        return _random.Next(max);
    }

    public int NextInt(int min, int max)
    {
        return _random.Next(min, max);
    }
}
