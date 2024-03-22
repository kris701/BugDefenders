using System.Collections;
using System.Reflection;
using System.Text.Json;

namespace BugDefender.Core.Models
{
    public class EffectTarget
    {
        public string Target { get; set; }
        private JsonElement? _value = null;
        public JsonElement? Value
        {
            get => _value;
            set
            {
                if (Modifier != 1 && value != null)
                    throw new Exception("Cannot have a modifier and value at the same time!");
                _value = value;
            }
        }
        private float _modifier = 1;
        public float Modifier
        {
            get => _modifier;
            set
            {
                if (Value != null && value != 1)
                    throw new Exception("Cannot have a modifier and value at the same time!");
                _modifier = value;
            }
        }

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

        public override bool Equals(object? obj)
        {
            if (obj is EffectTarget other)
            {
                if (other.Target != Target) return false;
                if (other.Modifier != Modifier) return false;
                if (!other.Value.Equals(Value)) return false;
                return true;
            }
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Target, Value, Modifier);

        public void ApplyUpgradeEffectOnObject<U>(U item) where U : notnull => ExecuteActionOnProperty(item, ApplyEffect);
        public void UnApplyUpgradeEffectOnObject<U>(U item) where U : notnull => ExecuteActionOnProperty(item, UnApplyEffect);
        public void TryApplyUpgradeEffectOnObject<U>(U item) where U : notnull => TryExecuteActionOnProperty(item, ApplyEffect);
        public void TryUnApplyUpgradeEffectOnObject<U>(U item) where U : notnull => TryExecuteActionOnProperty(item, UnApplyEffect);

        private void ApplyEffect<U>(U item, PropertyInfo first)
        {
            var current = first.GetValue(item);
            if (current == null)
                throw new Exception("Unknown change attempted on object!");
            if (Value != null)
            {
                var newValue = JsonSerializer.Deserialize((dynamic)Value, current.GetType());
                first.SetValue(item, newValue);
            }
            else if (Modifier != 1)
            {
                var asFloat = Convert.ToSingle(current);
                first.SetValue(item, Convert.ChangeType(asFloat * Modifier, current.GetType()));
            }
            else
                throw new Exception("Unknown change attempted on object!");
        }

        private void UnApplyEffect<U>(U item, PropertyInfo first)
        {
            var current = first.GetValue(item);
            if (current == null)
                throw new Exception("Unknown change attempted on object!");
            if (Modifier != 1)
            {
                var asFloat = Convert.ToSingle(current);
                first.SetValue(item, Convert.ChangeType(asFloat / Modifier, current.GetType()));
            }
            else
                throw new Exception("Unknown change attempted on object!");
        }

        private void ExecuteActionOnProperty<U>(U item, Action<object, PropertyInfo> action) where U : notnull
        {
            var target = GetPropertyInstance(item);
            if (target == null)
                throw new Exception("Invalid object selection!");
            action(target.Item1, target.Item2);
        }

        private void TryExecuteActionOnProperty<U>(U item, Action<object, PropertyInfo> action) where U : notnull
        {
            var target = GetPropertyInstance(item);
            if (target == null)
                return;
            action(target.Item1, target.Item2);
        }

        private Tuple<object, PropertyInfo>? GetPropertyInstance<U>(U item) where U : notnull
        {
            if (Target.Count(x => x == '[' || x == ']') % 2 != 0)
                return null;

            int seekIndex = 0;
            string parsed = "";
            PropertyInfo? targetInfo = null;
            object? targetObject = item;
            var propInfo = targetObject.GetType().GetProperties();
            while (seekIndex < Target.Length)
            {
                if (Target[seekIndex] == '.')
                {
                    targetInfo = propInfo.FirstOrDefault(x => x.Name == parsed);
                    if (targetInfo == null)
                        return null;
                    targetObject = targetInfo.GetValue(targetObject);
                    if (targetObject == null)
                        return null;
                    propInfo = targetObject.GetType().GetProperties();
                    parsed = "";
                }
                else if (Target[seekIndex] == '[')
                {
                    targetInfo = propInfo.FirstOrDefault(x => x.Name == parsed);
                    if (targetInfo == null)
                        return null;
                    targetObject = targetInfo.GetValue(targetObject);
                    if (targetObject == null)
                        return null;
                    propInfo = targetObject.GetType().GetProperties();

                    var within = Target.Substring(seekIndex + 1, Target.IndexOf(']', seekIndex) - seekIndex - 1);
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
                            return null;
                    }
                    else
                        return null;

                    parsed = "";
                    seekIndex += within.Length + 2;
                }
                else
                    parsed += Target[seekIndex];

                seekIndex++;
            }

            if (parsed != "")
                targetInfo = propInfo.FirstOrDefault(x => x.Name == parsed);

            if (targetObject != null && targetInfo != null)
                return new Tuple<object, PropertyInfo>(targetObject, targetInfo);
            return null;
        }
    }
}
