using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Frends.Community.JSON.EnforceJSONTypes.Tests
{
    [TestClass]
    public class JsonTypeEnforcerTest
    {
        [TestMethod]
        public void EnforceJsonTypesTest()
        {
            var json = @"{
  ""hello"": ""123"",
  ""world"": ""true"",
  ""bad_arr"": ""hello, world"",
  ""bad_arr_2"": { ""prop1"": 123 }
}";
            var result = JsonTypeEnforcer.EnforceJsonTypes(
                new EnforceJsonTypesParameters
                {
                    Json = json,
                    Rules = new[]
                    {
                        new JsonTypeRule("$.hello", JsonDataType.Number),
                        new JsonTypeRule("$.world", JsonDataType.Boolean),
                        new JsonTypeRule("$.bad_arr", JsonDataType.Array),
                        new JsonTypeRule("$.bad_arr_2", JsonDataType.Array),
                    }
                });
            var expected = @"{
  ""hello"": 123.0,
  ""world"": true,
  ""bad_arr"": [
    ""hello, world""
  ],
  ""bad_arr_2"": [
    {
      ""prop1"": 123
    }
  ]
}";
            Console.WriteLine(expected);
            Console.WriteLine(result);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ChangeDataTypeTest_Number()
        {
            JValue jValue;

            // Valid number
            jValue = new JValue("1.23");
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Number);
            Assert.AreEqual(1.23, jValue.Value);

            // Invalid number - do nothing
            jValue = new JValue("foo");
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Number);
            Assert.AreEqual("foo", jValue.Value);

            // Source is number - do nothing
            jValue = new JValue(1.23);
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Number);
            Assert.AreEqual(1.23, jValue.Value);
        }

        [TestMethod]
        public void ChangeDataTypeTest_Empty()
        {
            JValue jValue;

            // Empty - null
            jValue = new JValue("");
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Number);
            Assert.AreEqual(null, jValue.Value);

            // Empty - null
            jValue = new JValue((string) null);
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Number);
            Assert.AreEqual(null, jValue.Value);
        }

        [TestMethod]
        public void ChangeDataTypeTest_Booleans()
        {
            JValue jValue;

            // Valid bool
            jValue = new JValue("true");
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Boolean);
            Assert.AreEqual(true, jValue.Value);

            jValue = new JValue("TRUE");
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Boolean);
            Assert.AreEqual(true, jValue.Value);

            jValue = new JValue("True");
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Boolean);
            Assert.AreEqual(true, jValue.Value);

            jValue = new JValue("FaLsE");
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Boolean);
            Assert.AreEqual(false, jValue.Value);
            // Null bool

            jValue = new JValue((bool?) null);
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Boolean);
            Assert.AreEqual(null, jValue.Value);

            // Bool source
            jValue = new JValue(true);
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Boolean);
            Assert.AreEqual(true, jValue.Value);
        }

        [TestMethod]
        public void ChangeDataTypeTest_Arrays()
        {
            // Array
            var jObject = JObject.Parse(@"{
  ""arr"": 111
}");
            var jValue = (JValue)jObject.SelectTokens("$.arr").First();
            JsonTypeEnforcer.ChangeDataType(jValue, JsonDataType.Array);
            var jArray = (JArray) jObject.SelectToken("$.arr");
            Assert.AreEqual(1, jArray.Count);
            Assert.AreEqual(111, jArray[0]);
        }

        [TestMethod]
        public void ChangeDataTypeTest_ArraysWithComplexObjects()
        {
            // Array
            var jObject = JObject.Parse(@"{
  ""arr"": { ""prop1"": 111 }
}");
            var jToken = jObject.SelectTokens("$.arr").First();
            JsonTypeEnforcer.ChangeDataType(jToken, JsonDataType.Array);
            var jArray = (JArray)jObject.SelectToken("$.arr");
            Assert.AreEqual(1, jArray.Count);
            Assert.AreEqual(111, jArray[0]["prop1"].Value<int>());
        }


        [TestMethod]
        public void TestArrays()
        {
            var jObject = JObject.Parse(@"{
  ""hello"": ""123"",
  ""world"": ""true"",
  ""arr"": [1,2,3,4]
}");
            var tokens = jObject.SelectTokens("$.arr");
            var cnt = tokens.Count();
        }
    }
}
