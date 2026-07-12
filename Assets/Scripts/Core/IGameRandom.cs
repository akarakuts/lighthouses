using System;

namespace LighthouseMatch3
{
    public interface IGameRandom
    {
        int Range(int minInclusive, int maxExclusive);
    }

    public sealed class SeededGameRandom : IGameRandom
    {
        private readonly Random _random;

        public SeededGameRandom(int seed) => _random = new Random(seed);

        public int Range(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    }

    internal sealed class RandomAdapter : Random
    {
        private readonly IGameRandom _source;

        public RandomAdapter(IGameRandom source) => _source = source;

        public override int Next(int minValue, int maxValue) => _source.Range(minValue, maxValue);
    }
}
