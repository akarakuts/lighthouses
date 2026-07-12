using System.Collections.Generic;
using UnityEngine;

namespace LighthouseMatch3
{
    public sealed class ResourcesLevelCatalog : ILevelCatalog
    {
        private readonly Dictionary<int, LevelDefinition> _levels = new Dictionary<int, LevelDefinition>();
        private readonly ILevelCatalog _fallback;

        public ResourcesLevelCatalog(ILevelCatalog fallback = null)
        {
            _fallback = fallback ?? ProceduralLevelCatalog.Instance;
            LevelDefinitionAsset[] assets = Resources.LoadAll<LevelDefinitionAsset>("Levels");
            foreach (LevelDefinitionAsset asset in assets)
            {
                if (asset == null) continue;
                LevelDefinition definition = asset.ToDefinition();
                _levels[definition.Id] = definition;
            }
        }

        public LevelDefinition Get(int id) =>
            _levels.TryGetValue(id, out LevelDefinition definition) ? definition : _fallback.Get(id);
    }
}
