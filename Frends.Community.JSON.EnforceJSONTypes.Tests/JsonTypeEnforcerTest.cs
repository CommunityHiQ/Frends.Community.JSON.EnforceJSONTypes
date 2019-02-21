using System;
using System.Globalization;
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
}";
            var result = JsonTypeEnforcer.EnforceJsonTypes(
                new EnforceJsonTypesParameters
                {
                    Json = json,
                    Rules = new[]
                    {
                        new JsonTypeRule("$.hello", JsonDataType.Number),
                        new JsonTypeRule("$.world", JsonDataType.Boolean)
                    }
                });
            var expected = @"{
  ""hello"": 123.0,
  ""world"": true
}";
            Console.WriteLine(expected);
            Console.WriteLine(result);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ChangeDataTypeTest()
        {
            // Valid number
            Assert.AreEqual(1.23, JsonTypeEnforcer.ChangeDataType(new JValue("1.23"), JsonDataType.Number));
            // Invalid number - do nothing
            Assert.AreEqual("foo", JsonTypeEnforcer.ChangeDataType(new JValue("foo"), JsonDataType.Number));
            // Source is number - do nothing
            Assert.AreEqual(1.23, JsonTypeEnforcer.ChangeDataType(new JValue(1.23), JsonDataType.Number));

            // Empty - return null
            Assert.AreEqual(null, JsonTypeEnforcer.ChangeDataType(new JValue(""), JsonDataType.Number));
            // Empty - return null
            Assert.AreEqual(null, JsonTypeEnforcer.ChangeDataType(new JValue((string)null), JsonDataType.Number));

            // Valid bool
            Assert.AreEqual(true, JsonTypeEnforcer.ChangeDataType(new JValue("true"), JsonDataType.Boolean));
            Assert.AreEqual(true, JsonTypeEnforcer.ChangeDataType(new JValue("TRUE"), JsonDataType.Boolean));
            Assert.AreEqual(true, JsonTypeEnforcer.ChangeDataType(new JValue("True"), JsonDataType.Boolean));
            Assert.AreEqual(false, JsonTypeEnforcer.ChangeDataType(new JValue("FaLsE"), JsonDataType.Boolean));
            // Null bool
            Assert.AreEqual(null, JsonTypeEnforcer.ChangeDataType(new JValue((bool?)null), JsonDataType.Number));
            // Bool source
            Assert.AreEqual(true, JsonTypeEnforcer.ChangeDataType(new JValue(true), JsonDataType.Boolean));
        }
    }
}
