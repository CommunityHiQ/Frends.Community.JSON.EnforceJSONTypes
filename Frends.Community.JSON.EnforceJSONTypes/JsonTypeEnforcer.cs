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
        /// Documentation: https://github.com/CommunityHiQ/Frends.Community.JSON.EnforceJSONTypes
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <returns></returns>
        public static string EnforceJsonTypes(EnforceJsonTypesParameters parameters)
        {
            var jObject = JObject.Parse(parameters.Json);
            foreach (var rule in parameters.Rules)
            {
                foreach (var jValue in jObject.SelectTokens(rule.JsonPath).OfType<JValue>())
                {
                    
                    ChangeDataType(jValue, rule.DataType);
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
        internal static void ChangeDataType(JValue value, JsonDataType dataType)
        {
            object newValue = value.Value;
            try
            {
                switch (dataType)
                {
                    case JsonDataType.String:
                        newValue = value.Value<string>();
                        break;
                    case JsonDataType.Number:
                        if (value.Value == null || (value.Value as string) == "") newValue = null;
                        else newValue = value.Value<double>();
                        break;
                    case JsonDataType.Boolean:
                        if (value.Value == null || (value.Value as string) == "") newValue = null;
                        else newValue = value.Value<bool>();
                        break;
                    case JsonDataType.Array:
                        // Here we actually need to replace the JValue with a JArray that would contain the current JValue
                        var jProperty = value.Parent as JProperty;
                        if (jProperty != null)
                        {
                            var jArray = new JArray();
                            jArray.Add(value);
                            jProperty.Value = jArray;
                        }

                        break;
                    default:
                        throw new Exception($"Unknown JSON data type {dataType}");
                }

                if (dataType != JsonDataType.Array) value.Value = newValue;
            }
            catch
            {
                // do nothing
            }
        }
    }
}
