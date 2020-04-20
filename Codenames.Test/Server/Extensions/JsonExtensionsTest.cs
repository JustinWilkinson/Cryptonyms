using Codenames.Server.Extensions;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Codenames.Test.Server.Extensions
{
    public class JsonExtensionsTest
    {

        private static readonly TestSerializerClass _testObject = new TestSerializerClass { IntValue = 1, StringValue = "Value", camelCaseStringValue = "value" };
        private static readonly JsonElement _objectJsonElement = JsonDocument.Parse(JsonSerializer.Serialize(_testObject)).RootElement;

        private static readonly TestSerializerClass _complexTestObject = new TestSerializerClass { IntValue = 1, StringValue = JsonConvert.SerializeObject(_testObject), camelCaseStringValue = JsonConvert.SerializeObject(_testObject) };
        private static readonly JsonElement _complexObjectJsonElement = JsonDocument.Parse(JsonSerializer.Serialize(_complexTestObject)).RootElement;

        [Test]
        public void GetStringProperty_ValidJsonElementObjectPascalCase_ExtractsStringPropertySuccessfully()
        {
            Assert.AreEqual("Value", _objectJsonElement.GetStringProperty("StringValue"));
        }

        [Test]
        public void GetStringProperty_ValidJsonElementObjectCamelCase_ExtractsStringPropertySuccessfully()
        {
            Assert.AreEqual("value", _objectJsonElement.GetStringProperty("camelCaseStringValue"));
            Assert.AreEqual("value", _objectJsonElement.GetStringProperty("CamelCaseStringValue"));
        }

        [Test]
        public void DeserializeStringProperty_ValidJsonElementObject_DeserializesStringProperty()
        {
            // Arrange/Act
            var result = _complexObjectJsonElement.DeserializeStringProperty<TestSerializerClass>("StringValue");

            // Assert
            Assert.IsInstanceOf<TestSerializerClass>(result);
            Assert.IsTrue(result.ValueEquals(_testObject));
        }

        [Test]
        public void DeserializeStringProperty_ValidJsonElementObject_DeserializesStringPropertyCamelCase()
        {
            // Arrange/Act
            var result = _complexObjectJsonElement.DeserializeStringProperty<TestSerializerClass>("camelCaseStringValue");
            var result2 = _complexObjectJsonElement.DeserializeStringProperty<TestSerializerClass>("CamelCaseStringValue");

            // Assert
            Assert.IsInstanceOf<TestSerializerClass>(result);
            Assert.IsInstanceOf<TestSerializerClass>(result2);
            Assert.IsTrue(result.ValueEquals(_testObject));
            Assert.IsTrue(result2.ValueEquals(_testObject));
        }

        [Test]
        public void DeserializeStringProperty_ValidJsonElementString_RetrievesPropertyFromDeserializedString()
        {
            // Arrange
            var element = JsonDocument.Parse(JsonSerializer.Serialize(JsonConvert.SerializeObject(new { Object = _testObject }))).RootElement;

            // Act
            var result = element.DeserializeStringProperty<TestSerializerClass>("Object");

            // Assert
            Assert.IsInstanceOf<TestSerializerClass>(result);
            Assert.IsTrue(result.ValueEquals(_testObject));
        }

        private class TestSerializerClass
        {
            public int IntValue { get; set; }

            public string StringValue { get; set; }

            public string camelCaseStringValue { get; set; }

            public bool ValueEquals(TestSerializerClass other) => IntValue == other.IntValue && StringValue == other.StringValue && camelCaseStringValue == other.camelCaseStringValue;
        }
    }
}