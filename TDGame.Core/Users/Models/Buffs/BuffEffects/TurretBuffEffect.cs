using System.Text.Json.Serialization;
using TDGame.Core.Game.Models.Entities.Turrets.Modules;

namespace TDGame.Core.Users.Models.Buffs.BuffEffects
{
    public class TurretBuffEffect : IBuffEffect
    {
        public Guid TurretID { get; set; }
        public ITurretModule Module { get; set; }

        [JsonIgnore]
        public Guid BuffID { get; set; }
    }
}
