using Cryptonyms.Server.Extensions;
using NUnit.Framework;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Cryptonyms.Test.Server.Extensions
{
    [TestFixture]
    public class JsonExtensionsTest
    {

        private static readonly TestSerializerClass _testObject = new TestSerializerClass { IntValue = 1, StringValue = "Value", BooleanValue = true, camelCaseStringValue = "value", camelCaseBooleanValue = false };
        private static readonly JsonElement _objectJsonElement = JsonDocument.Parse(JsonSerializer.Serialize(_testObject)).RootElement;

        private static readonly TestWithObjectProperty _testWithObjectProperty = new TestWithObjectProperty { ObjectProperty = _testObject, camelCaseObjectProperty = _testObject };
        private static readonly JsonElement _testWithObjectPropertyJsonElement = JsonDocument.Parse(JsonSerializer.Serialize(_testWithObjectProperty)).RootElement;

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
        public void GetObjectProperty_ValidJsonElementObjectPascalCase_ExtractsObjectPropertySuccessfully()
        {
            var result = _testWithObjectPropertyJsonElement.GetObjectProperty<TestSerializerClass>("ObjectProperty");
            Assert.IsInstanceOf<TestSerializerClass>(result);
            Assert.IsTrue(result.ValueEquals(_testObject));
        }

        [Test]
        public void GetObjectProperty_ValidJsonElementObjectCamelCase_ExtractsObjectPropertySuccessfully()
        {
            var result1 = _testWithObjectPropertyJsonElement.GetObjectProperty<TestSerializerClass>("camelCaseObjectProperty");
            var result2 = _testWithObjectPropertyJsonElement.GetObjectProperty<TestSerializerClass>("CamelCaseObjectProperty");

            Assert.IsInstanceOf<TestSerializerClass>(result1);
            Assert.IsTrue(result1.ValueEquals(_testObject));
            Assert.IsInstanceOf<TestSerializerClass>(result2);
            Assert.IsTrue(result2.ValueEquals(_testObject));
        }

        [Test]
        public void GetBooleanProperty_ValidJsonElementObjectPascalCase_ExtractsBooleanPropertySuccessfully()
        {
            Assert.AreEqual(true, _objectJsonElement.GetBooleanProperty("BooleanValue"));
        }

        [Test]
        public void GetBooleanProperty_ValidJsonElementObjectCamelCase_ExtractsBooleanPropertySuccessfully()
        {
            Assert.AreEqual(false, _objectJsonElement.GetBooleanProperty("camelCaseBooleanValue"));
            Assert.AreEqual(false, _objectJsonElement.GetBooleanProperty("CamelCaseBooleanValue"));
        }

        private class TestSerializerClass
        {
            public int IntValue { get; set; }

            public string StringValue { get; set; }

            public bool BooleanValue { get; set; }

            public bool camelCaseBooleanValue { get; set; }

            public string camelCaseStringValue { get; set; }

            public bool ValueEquals(TestSerializerClass other)
            {
                return IntValue == other.IntValue && StringValue == other.StringValue && camelCaseStringValue == other.camelCaseStringValue;
            }
        }

        private class TestWithObjectProperty
        {
            public object ObjectProperty { get; set; }

            public object camelCaseObjectProperty { get; set; }
        }
    }
}