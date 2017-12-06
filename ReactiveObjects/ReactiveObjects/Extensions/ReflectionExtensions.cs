using System.Reflection;

namespace ReactiveObjects.Extensions
{
    public static class ReflectionExtensions {
        public static object GetValue(this object value, string propertyName) {
            PropertyInfo property = value
                .GetType()
                .GetProperty(propertyName);

            object result = property?.GetValue(value);

            return result;
        }
    }
}