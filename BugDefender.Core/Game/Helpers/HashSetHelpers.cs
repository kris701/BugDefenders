namespace BugDefender.Core.Game.Helpers
{
    public static class HashSetHelpers
    {
        public static void AddRange<T>(this HashSet<T> to, HashSet<T> with)
        {
            foreach (var item in with)
                to.Add(item);
        }
    }
}
