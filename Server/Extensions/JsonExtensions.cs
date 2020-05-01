using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;

namespace Cryptonyms.Server.Extensions
{
    /// <summary>
    /// Contains useful extensions to both System.Text.Json using Newtonsoft Json.
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Uses Newtonsoft Json to deserialize a string to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="str">String to deserialize</param>
        /// <returns>A new instance of T from the string representation provided.</returns>
        public static T Deserialize<T>(this string str) => JsonConvert.DeserializeObject<T>(str);

        /// <summary>
        /// Converts a JsonElement to a string, and then uses Newtonsoft Json to deserialize it to the specified type.
        /// </summary>
        /// <typeparam name="T">Type of object to deserialize to</typeparam>
        /// <param name="json">JsonElement to deserialize</param>
        /// <returns>A new instance of T from the string representation provided.</returns>
        public static T Deserialize<T>(this JsonElement json) => json.GetString().Deserialize<T>();

        /// <summary>
        /// Extracts the value of a JsonElement's property as the specified type - this check is case insensitive.
        /// </summary>
        /// <typeparam name="T">Type to extract property as</typeparam>
        /// <param name="json">JsonElement with the required property</param>
        /// <returns>A new instance of T extracted from the JsonElement property.</returns>
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

        /// <summary>
        /// Extracts the value of a JsonElement's property as the specified type - this check is case insensitive.
        /// </summary>
        /// <typeparam name="T">Type to extract property as</typeparam>
        /// <param name="json">JsonElement with the required property</param>
        /// <returns>A new instance of T extracted from the JsonElement property.</returns>
        public static T GetObjectProperty<T>(this JsonElement json, string propertyName, Func<JsonElement, T> propertyConverter)
        {
            if (json.TryGetProperty(propertyName, out var property))
            {
                return propertyConverter(property);
            }
            else
            {
                propertyName = propertyName.Length > 1 ? $"{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}" : propertyName.ToLowerInvariant();
                return json.TryGetProperty(propertyName, out property) ? propertyConverter(property) : default;
            }
        }

        /// <summary>
        /// Extracts the value of a JsonElement's property as a string this check is case insensitive.
        /// </summary>
        /// <param name="json">JsonElement with the required property</param>
        /// <returns>The string value of the specified property.</returns>
        public static string GetStringProperty(this JsonElement json, string propertyName) => GetObjectProperty(json, propertyName, property => property.GetString());

        /// <summary>
        /// Extracts the value of a JsonElement's property as a boolean this check is case insensitive.
        /// </summary>
        /// <param name="json">JsonElement with the required property</param>
        /// <returns>The boolean value of the specified property.</returns>
        public static bool GetBooleanProperty(this JsonElement json, string propertyName) => GetObjectProperty(json, propertyName, property => property.GetBoolean());

        /// <summary>
        /// Serializes the object provided using Newtonsoft Json and an indented format for easy readability.
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>A json string respresentaion of the provided object</returns>
        public static string Serialize(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}