using System.Text.Json;

namespace BugDefender.Core.Users.Models.Buffs.BuffEffects
{
    public class ChangeTarget
    {
        public string Target { get; set; }
        public JsonElement? Value { get; set; } = null;
        public float Modifier { get; set; } = 1;

        public ChangeTarget(string target)
        {
            Target = target;
        }
    }
}
