namespace LighthouseMatch3
{
    public static class BlockerRules
    {
        public static int RequiredHits(BlockerKind blocker)
        {
            return blocker == BlockerKind.Crate ? 2 : blocker == BlockerKind.None ? 0 : 1;
        }

        public static bool CanBeDamaged(BlockerState blocker, bool triggeredBySpecial)
        {
            return blocker != null && blocker.Kind != BlockerKind.None
                && (blocker.Kind != BlockerKind.Seaweed || triggeredBySpecial);
        }

        public static bool Damage(BlockerState blocker, bool triggeredBySpecial)
        {
            if (!CanBeDamaged(blocker, triggeredBySpecial)) return false;
            if (blocker.HitsRemaining <= 0) blocker.HitsRemaining = RequiredHits(blocker.Kind);

            blocker.HitsRemaining--;
            if (blocker.HitsRemaining > 0) return false;

            blocker.Kind = BlockerKind.None;
            blocker.HitsRemaining = 0;
            return true;
        }
    }
}
