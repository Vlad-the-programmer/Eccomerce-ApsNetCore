using System.Reflection;

namespace EcommerceRestApi.Helpers.ModelsUtils
{

    public static class PropertyUtil
    {
        public static T CopyProperties<T, T2>(this T targetObject, T2 sourceObject)
        {
            if (sourceObject == null || targetObject == null)
                return targetObject;

            // Get all simple properties (value types and strings) from the actual runtime types
            var targetProperties = targetObject.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && IsDirectlyAssignable(p.PropertyType))
                .ToList();

            var sourceProperties = sourceObject.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && IsDirectlyAssignable(p.PropertyType))
                .ToList();

            foreach (var targetProp in targetProperties)
            {
                var sourceProp = sourceProperties.FirstOrDefault(p =>
                    string.Equals(p.Name, targetProp.Name, StringComparison.OrdinalIgnoreCase) &&
                    p.PropertyType == targetProp.PropertyType);

                if (sourceProp != null)
                {
                    try
                    {
                        var value = sourceProp.GetValue(sourceObject);
                        targetProp.SetValue(targetObject, value);
                    }
                    catch { /* Skip property if there's an error */ }
                }
            }

            return targetObject;
        }

        private static bool IsDirectlyAssignable(Type type)
        {
            if (type == null) return false;

            // Handle nullable value types
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                type = underlyingType;
            }

            return type.IsPrimitive
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(TimeSpan)
                || type == typeof(Guid)
                || type.IsEnum;
        }
    }
}