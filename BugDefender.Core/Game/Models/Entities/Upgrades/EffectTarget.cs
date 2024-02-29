using System.Text.Json;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
{
    public class EffectTarget
    {
        public string Target { get; set; }
        public JsonElement? Value { get; set; } = null;
        public float Modifier { get; set; } = 1;

        public EffectTarget(string target)
        {
            Target = target;
        }
    }
}
