using BugDefender.Core.Game.Models.Entities.Turrets.Modules;
using BugDefender.Core.Users.Models.Buffs;
using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BugDefender.Core.Game.Models.Entities.Upgrades
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
                ExecuteActionOnProperty(effect, item, ApplyEffect);
        }

        public void TryApplyUpgradeEffectOnObject<U>(U item) where U : notnull
        {
            foreach (var effect in Effects)
                TryExecuteActionOnProperty(effect, item, ApplyEffect);
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

        public void UnApplyUpgradeEffectOnObject<U>(U item) where U : notnull
        {
            foreach (var effect in Effects)
                ExecuteActionOnProperty(effect, item, UnApplyEffect);
        }

        public void TryUnApplyUpgradeEffectOnObject<U>(U item) where U : notnull
        {
            foreach (var effect in Effects)
                TryExecuteActionOnProperty(effect, item, UnApplyEffect);
        }

        private void ExecuteActionOnProperty<U>(EffectTarget effect, U item, Action<object, PropertyInfo, EffectTarget> action) where U : notnull
        {
            if (effect.Target.Count(x => x == '[' || x == ']') % 2 != 0)
                throw new Exception("Missmatch of list accessors!");

            int seekIndex = 0;
            string parsed = "";
            PropertyInfo? targetInfo = null;
            object? targetObject = item;
            var propInfo = targetObject.GetType().GetProperties();
            while (seekIndex < effect.Target.Length)
            {
                if (effect.Target[seekIndex] == '.')
                {
                    targetInfo = propInfo.First(x => x.Name == parsed);
                    targetObject = targetInfo.GetValue(targetObject);
                    if (targetObject == null)
                        throw new Exception("Invalid object selection!");
                    propInfo = targetObject.GetType().GetProperties();
                    parsed = "";
                }
                else if (effect.Target[seekIndex] == '[')
                {
                    targetInfo = propInfo.First(x => x.Name == parsed);
                    targetObject = targetInfo.GetValue(targetObject);
                    if (targetObject == null)
                        throw new Exception("Invalid object selection!");
                    propInfo = targetObject.GetType().GetProperties();

                    var within = effect.Target.Substring(seekIndex + 1, effect.Target.IndexOf(']', seekIndex) - seekIndex - 1);
                    var split = within.Split('=');
                    var targetProp = split[0];
                    var targetValue = split[1];

                    if (targetObject is IEnumerable enu)
                    {
                        bool any = false;
                        foreach(var inner in enu)
                        {
                            var listPropInfo = inner.GetType().GetProperties();
                            var listTargetInfo = listPropInfo.First(x => x.Name == targetProp);
                            var listTargetObject = listTargetInfo.GetValue(inner);
                            if (listTargetObject == null)
                                continue;
                            if (listTargetObject.ToString() == targetValue)
                            {
                                targetInfo = null;
                                propInfo = listPropInfo;
                                targetObject = inner;
                                any = true;
                                break;
                            }
                        }
                        if (!any)
                            throw new Exception("Nothing in the collection matched the target!");
                    }
                    else
                        throw new Exception("Attempted to access a collectible but the property is not an instance of a collectible!");

                    parsed = "";
                    seekIndex += within.Length + 2;
                }
                else
                    parsed += effect.Target[seekIndex];

                seekIndex++;
            }

            if (parsed != "")
                targetInfo = propInfo.First(x => x.Name == parsed);

            if (targetObject != null && targetInfo != null)
                action(targetObject, targetInfo, effect);
        }

        private void TryExecuteActionOnProperty<U>(EffectTarget effect, U item, Action<object, PropertyInfo, EffectTarget> action) where U : notnull
        {
            if (effect.Target.Count(x => x == '[' || x == ']') % 2 != 0)
                return;

            int seekIndex = 0;
            string parsed = "";
            PropertyInfo? targetInfo = null;
            object? targetObject = item;
            var propInfo = targetObject.GetType().GetProperties();
            while (seekIndex < effect.Target.Length)
            {
                if (effect.Target[seekIndex] == '.')
                {
                    targetInfo = propInfo.FirstOrDefault(x => x.Name == parsed);
                    if (targetInfo == null)
                        return;
                    targetObject = targetInfo.GetValue(targetObject);
                    if (targetObject == null)
                        return;
                    propInfo = targetObject.GetType().GetProperties();
                    parsed = "";
                }
                else if (effect.Target[seekIndex] == '[')
                {
                    targetInfo = propInfo.FirstOrDefault(x => x.Name == parsed);
                    if (targetInfo == null)
                        return;
                    targetObject = targetInfo.GetValue(targetObject);
                    if (targetObject == null)
                        return;
                    propInfo = targetObject.GetType().GetProperties();

                    var within = effect.Target.Substring(seekIndex + 1, effect.Target.IndexOf(']', seekIndex) - seekIndex - 1);
                    var split = within.Split('=');
                    var targetProp = split[0];
                    var targetValue = split[1];

                    if (targetObject is IEnumerable enu)
                    {
                        bool any = false;
                        foreach (var inner in enu)
                        {
                            var listPropInfo = inner.GetType().GetProperties();
                            var listTargetInfo = listPropInfo.FirstOrDefault(x => x.Name == targetProp);
                            if (listTargetInfo == null)
                                continue;
                            var listTargetObject = listTargetInfo.GetValue(inner);
                            if (listTargetObject == null)
                                continue;
                            if (listTargetObject.ToString() == targetValue)
                            {
                                targetInfo = null;
                                propInfo = listPropInfo;
                                targetObject = inner;
                                any = true;
                                break;
                            }
                        }
                        if (!any)
                            return;
                    }
                    else
                        return;

                    parsed = "";
                    seekIndex += within.Length + 2;
                }
                else
                    parsed += effect.Target[seekIndex];

                seekIndex++;
            }

            if (parsed != "")
                targetInfo = propInfo.FirstOrDefault(x => x.Name == parsed);

            if (targetObject != null && targetInfo != null)
                action(targetObject, targetInfo, effect);
        }

        private void UnApplyEffect<U>(U item, PropertyInfo first, EffectTarget effect)
        {
            var current = first.GetValue(item);
            if (current == null)
                throw new Exception("Unknown change attempted on object!");
            if (effect.Modifier != 1)
            {
                var asFloat = Convert.ToSingle(current);
                first.SetValue(item, Convert.ChangeType(asFloat / effect.Modifier, current.GetType()));
            }
            else
                throw new Exception("Unknown change attempted on object!");
        }
    }
}
