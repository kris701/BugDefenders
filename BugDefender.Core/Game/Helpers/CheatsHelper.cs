﻿namespace BugDefender.Core.Game.Helpers
{
    public enum CheatTypes { None, InfiniteMoney, MaxWaves, DamageX10, Invincibility, EnemiesX100 }
    public static class CheatsHelper
    {
        private static readonly Dictionary<string, CheatTypes> _cheats = new Dictionary<string, CheatTypes>()
        {
            { "motherload", CheatTypes.InfiniteMoney },
            { "maxwave", CheatTypes.MaxWaves },
            { "destruction", CheatTypes.DamageX10 },
            { "nokillplz", CheatTypes.Invincibility },
            { "iwanttodie", CheatTypes.EnemiesX100 }
        };

        public static HashSet<CheatTypes> Cheats { get; set; } = new HashSet<CheatTypes>();

        public static void AddCheat(string text)
        {
            if (_cheats.ContainsKey(text))
            {
                if (Cheats.Contains(_cheats[text]))
                    Cheats.Remove(_cheats[text]);
                else
                    Cheats.Add(_cheats[text]);
            }
        }
    }
}
