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

        private string? _displayName;
        public string GetDisplayName()
        {
            if (_displayName != null)
                return _displayName;
            _displayName = Target;
            if (_displayName.Contains('.'))
                _displayName = _displayName.Substring(_displayName.LastIndexOf('.') + 1);
            for(int i = 0; i < _displayName.Length; i++)
            {
                if (char.IsUpper(_displayName[i]))
                {
                    _displayName = _displayName.Insert(i, " ");
                    i++;
                }
            }
            _displayName = _displayName.Trim();
            return _displayName;
        }
    }
}
