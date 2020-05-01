using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace Cryptonyms.Server.Extensions
{
    public static class JsonExtensions
    {
        public static T Deserialize<T>(this string str) => JsonConvert.DeserializeObject<T>(str);

        public static T Deserialize<T>(this JsonElement json) => json.GetString().Deserialize<T>();

        public static T GetObjectProperty<T>(this JsonElement json, string propertyName)
        {
            if (json.ValueKind == JsonValueKind.Object)
            {
                if (json.TryGetProperty(propertyName, out var property))
                {
                    return JsonConvert.DeserializeObject<T>(property.ToString());
                }
                else
                {
                    propertyName = propertyName.Length > 1 ? $"{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}" : propertyName.ToLowerInvariant();
                    return json.TryGetProperty(propertyName, out property) ? JsonConvert.DeserializeObject<T>(property.ToString()) : default;
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(JObject.Parse(json.GetString()).GetValue(propertyName)));
            }
        }

        public static T DeserializeStringProperty<T>(this JsonElement json, string propertyName)
        {
            if (json.ValueKind == JsonValueKind.Object)
            {
                if (json.TryGetProperty(propertyName, out var property))
                {
                    return JsonConvert.DeserializeObject<T>(property.GetString());
                }
                else
                {
                    propertyName = propertyName.Length > 1 ? $"{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}" : propertyName.ToLowerInvariant();
                    return json.TryGetProperty(propertyName, out property) ? JsonConvert.DeserializeObject<T>(property.GetString()) : default;
                }
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(JObject.Parse(json.GetString()).GetValue(propertyName)));
            }
        }

        public static string GetStringProperty(this JsonElement json, string propertyName)
        {
            if (json.TryGetProperty(propertyName, out var property))
            {
                return property.GetString();
            }
            else
            {
                propertyName = propertyName.Length > 1 ? $"{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}" : propertyName.ToLowerInvariant();
                return json.TryGetProperty(propertyName, out property) ? property.GetString() : null;
            }
        }

        public static bool GetBooleanProperty(this JsonElement json, string propertyName)
        {
            if (json.TryGetProperty(propertyName, out var property))
            {
                return property.GetBoolean();
            }
            else
            {
                propertyName = propertyName.Length > 1 ? $"{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}" : propertyName.ToLowerInvariant();
                return json.TryGetProperty(propertyName, out property) ? property.GetBoolean() : false;
            }
        }

        public static string Serialize(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}