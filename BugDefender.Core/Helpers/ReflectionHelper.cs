using System.Collections;
using System.Reflection;

namespace BugDefender.Core.Helpers
{
    public static class ReflectionHelper
    {
        public static Tuple<object, PropertyInfo>? GetPropertyInstance<U>(U item, string target) where U : notnull
        {
            if (target.Count(x => x == '[' || x == ']') % 2 != 0)
                return null;

            int seekIndex = 0;
            string parsed = "";
            PropertyInfo? targetInfo = null;
            object? targetObject = item;
            var propInfo = targetObject.GetType().GetProperties();
            while (seekIndex < target.Length)
            {
                if (target[seekIndex] == '.')
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
                else if (target[seekIndex] == '[')
                {
                    targetInfo = propInfo.FirstOrDefault(x => x.Name == parsed);
                    if (targetInfo == null)
                        return null;
                    targetObject = targetInfo.GetValue(targetObject);
                    if (targetObject == null)
                        return null;
                    propInfo = targetObject.GetType().GetProperties();

                    var within = target.Substring(seekIndex + 1, target.IndexOf(']', seekIndex) - seekIndex - 1);
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
                    parsed += target[seekIndex];

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
