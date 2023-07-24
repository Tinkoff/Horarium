using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Cronos;
using Newtonsoft.Json;

[assembly:InternalsVisibleTo("Horarium.Test")]
namespace Horarium
{
    internal static class Utils
    {
        public static string ToJson(this object obj, Type type, JsonSerializerSettings jsonSerializerSettings)
        {
           return JsonConvert.SerializeObject(obj, type, jsonSerializerSettings);
        }

        public static object FromJson(this string json, Type type, JsonSerializerSettings jsonSerializerSettings)
        {
            return JsonConvert.DeserializeObject(json, type, jsonSerializerSettings);
        }

        public static string AssemblyQualifiedNameWithoutVersion(this Type type)
        {
            if (string.IsNullOrWhiteSpace(type.FullName))
            {
                throw new ArgumentException($"Unable to receive FullName for type {type}");
            }

            if (!type.IsGenericType)
            {
                return $"{type.FullName}, {type.GetTypeInfo().Assembly.GetName().Name}";
            }

            var genericArguments = type
                .GetGenericArguments()
                .Select(typeArgument => $"[{typeArgument.AssemblyQualifiedNameWithoutVersion()}]")
                .ToArray();

            var genericPart = string.Join(", ", genericArguments);

            var bracketIndex = type.FullName.IndexOf("[", StringComparison.Ordinal);

            return $"{type.FullName.Substring(0, bracketIndex)}[{genericPart}], {type.GetTypeInfo().Assembly.GetName().Name}";
        }

        public static DateTime? ParseAndGetNextOccurrence(string cron)
        {
            var expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);

            return expression.GetNextOccurrence(DateTime.UtcNow, TimeZoneInfo.Local);
        }
    }
}