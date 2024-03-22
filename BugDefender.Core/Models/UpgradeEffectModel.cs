using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Game.Models.Entities.Upgrades;
using BugDefender.Core.Users.Models.Buffs;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Models
{
    [JsonDerivedType(typeof(UpgradeDefinition))]
    [JsonDerivedType(typeof(BuffEffect))]
    [JsonDerivedType(typeof(PassiveTurretDefinition))]
    public class UpgradeEffectModel
    {
        public List<EffectTarget> Effects { get; set; }

        public UpgradeEffectModel(List<EffectTarget> effects)
        {
            Effects = effects;
        }

        public UpgradeEffectModel Copy()
        {
            var newList = new List<EffectTarget>();
            foreach (var effect in Effects)
                newList.Add(effect.Copy());
            return new UpgradeEffectModel(newList);
        }

        public void ApplyUpgradeEffectOnObject<U>(U item) where U : notnull
        {
            foreach (var effect in Effects)
                effect.ApplyOnObject(item);
        }

        public void TryApplyUpgradeEffectOnObject<U>(U item) where U : notnull
        {
            foreach (var effect in Effects)
                effect.TryApplyOnObject(item);
        }

        public void UnApplyUpgradeEffectOnObject<U>(U item) where U : notnull
        {
            foreach (var effect in Effects)
                effect.UnApplyOnObject(item);
        }

        public void TryUnApplyUpgradeEffectOnObject<U>(U item) where U : notnull
        {
            foreach (var effect in Effects)
                effect.TryUnApplyOnObject(item);
        }
    }
}
