using System.Collections.Generic;

namespace ReactiveObjects.Extensions
{
    public static class StringExtensions
    {
        public static string Join<T>(this IEnumerable<T> collection, string separator)
        {
            return string.Join(separator, collection);
        }
    }
}