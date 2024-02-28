using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
