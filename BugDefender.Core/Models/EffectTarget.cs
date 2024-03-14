﻿using System.Text.Json;

namespace BugDefender.Core.Models
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

        public EffectTarget Copy()
        {
            if (Value != null)
                return new EffectTarget(Target) { Value = Value };
            else
                return new EffectTarget(Target) { Modifier = Modifier };
        }

        private string? _displayName;
        public override string ToString()
        {
            if (_displayName != null)
                return _displayName;
            _displayName = Target;
            if (_displayName.Contains('.'))
                _displayName = _displayName.Substring(_displayName.LastIndexOf('.') + 1);
            for (int i = 0; i < _displayName.Length; i++)
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