using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Frends.Community.JSON.EnforceJSONTypes
{
    /// <summary>
    /// JsonTypeEnforcer contains the EnforceJsonTypes task
    /// </summary>
    public class JsonTypeEnforcer
    {
        /// <summary>
        /// This task allows enforcing types in JSON documents by giving an array of
        /// JSON paths and corresponding JSON types.
        /// </summary>
        /// <param name="json">JSON document</param>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public static string EnforceJsonTypes(string json, EnforceJsonTypesParameters parameters)
        {
            var jObject = JObject.Parse(json);
            foreach (var rule in parameters.Rules)
            {
                foreach (var jValue in jObject.SelectTokens(rule.JsonPath).OfType<JValue>())
                {
                    jValue.Value = ChangeDataType(jValue, rule.DataType);
                }
            }

            return jObject.ToString();
        }

        /// <summary>
        /// Changes value of JValue object to the desired JSON data type
        /// </summary>
        /// <param name="value">JValue to change</param>
        /// <param name="dataType">New data type</param>
        /// <returns></returns>
        internal static object ChangeDataType(JValue value, JsonDataType dataType)
        {
            try
            {
                switch (dataType)
                {
                    case JsonDataType.String:
                        return value.Value<string>();
                    case JsonDataType.Number:
                        if (value.Value == null || (value.Value as string) == "") return null;
                        return value.Value<double>();
                    case JsonDataType.Boolean:
                        if (value.Value == null || (value.Value as string) == "") return null;
                        return value.Value<bool>();
                    default:
                        throw new Exception($"Unknown JSON data type {dataType}");
                }
            }
            catch
            {
                return value.Value;
            }
        }
    }
}
