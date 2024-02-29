using BugDefender.Core.Users.Models.Buffs;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
{
    [JsonDerivedType(typeof(UpgradeDefinition))]
    [JsonDerivedType(typeof(BuffEffect))]
    public class UpgradeEffectModel
    {
        public List<EffectTarget> Effects { get; set; }

        public UpgradeEffectModel(List<EffectTarget> effects)
        {
            Effects = effects;
        }

        public void ApplyUpgradeEffectOnObject<U>(U item) where U : notnull
        {
            var propInfo = item.GetType().GetProperties();
            foreach (var effect in Effects)
            {
                if (effect.Target.Contains('.'))
                {
                    PropertyInfo? targetInfo = null;
                    object? targetObject = item;
                    var targetText = effect.Target;
                    var innerPropInfo = targetObject.GetType().GetProperties();
                    while (targetText.Contains('.'))
                    {
                        var split = targetText.Split('.');
                        targetInfo = innerPropInfo?.First(x => x.Name == split[0]);
                        targetObject = targetInfo?.GetValue(targetObject);
                        innerPropInfo = targetObject?.GetType().GetProperties();
                        targetText = targetText.Substring(targetText.IndexOf('.') + 1);
                    }
                    targetInfo = innerPropInfo?.First(x => x.Name == targetText);
                    if (targetObject == null || targetInfo == null)
                        throw new Exception("Unknown change attempted on object!");
                    ApplyEffect(targetObject, targetInfo, effect);
                }
                else
                {
                    var first = propInfo.First(x => x.Name == effect.Target);
                    if (first != null)
                        ApplyEffect(item, first, effect);
                    else
                        throw new Exception("Unknown change attempted on object!");
                }
            }
        }

        private void ApplyEffect<U>(U item, PropertyInfo first, EffectTarget effect)
        {
            var current = first.GetValue(item);
            if (current == null)
                throw new Exception("Unknown change attempted on object!");
            if (effect.Value != null)
            {
                var newValue = JsonSerializer.Deserialize((dynamic)effect.Value, current.GetType());
                first.SetValue(item, newValue);
            }
            else if (effect.Modifier != 1)
            {
                var asFloat = Convert.ToSingle(current);
                first.SetValue(item, Convert.ChangeType(asFloat * effect.Modifier, current.GetType()));
            }
            else
                throw new Exception("Unknown change attempted on object!");
        }
    }
}
