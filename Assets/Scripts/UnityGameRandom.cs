namespace LighthouseMatch3
{
    public sealed class UnityGameRandom : IGameRandom
    {
        public int Range(int minInclusive, int maxExclusive) =>
            UnityEngine.Random.Range(minInclusive, maxExclusive);
    }
}
