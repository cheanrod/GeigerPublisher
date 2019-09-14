using System;
using NUnit.Framework;
using GeigerPublisher.Values;

namespace GeigerPublisher.Tests
{
    public class GeigerPublisherConverterTests
    {
       [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void ConvertWithValidString()
        {
            var reading = "CPS, 2, CPM, 42, uSv/hr, 0.03, SLOW";

            // Test
            var result = GeigerValuesConverter.ConvertFromReading(reading);

            // Verification
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CPS, Is.EqualTo(2));
            Assert.That(result.CPM, Is.EqualTo(42));
            Assert.That(result.Radiation, Is.EqualTo(0.03));
            Assert.That(result.Mode, Is.EqualTo(ModeEnum.SLOW));
        }

        [Test]
        public void ConvertWithValidString2()
        {
            var reading = "CPS, 42, CPM, 4242, uSv/hr, 100.03, FAST";

            // Test
            var result = GeigerValuesConverter.ConvertFromReading(reading);

            // Verification
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CPS, Is.EqualTo(42));
            Assert.That(result.CPM, Is.EqualTo(4242));
            Assert.That(result.Radiation, Is.EqualTo(100.03));
            Assert.That(result.Mode, Is.EqualTo(ModeEnum.FAST));
        }

        [Test]
        public void ConvertWithInvalidCPS()
        {
            var reading = "CPS, invalid, CPM, 42, uSv/hr, 0.03, SLOW";

            // Test & Verification
            Assert.That(() => { GeigerValuesConverter.ConvertFromReading(reading); }, Throws.TypeOf<FormatException>());
        }

        [Test]
        public void ConvertWithInvalidMode()
        {
            var reading = "CPS, invalid, CPM, 42, uSv/hr, 0.03, FOO";

            // Test & Verification
            Assert.That(() => { GeigerValuesConverter.ConvertFromReading(reading); }, Throws.TypeOf<FormatException>());
        }

        [Test]
        public void ConvertJSONValid()
        {
            var value = new GeigerValues
            {
                CPS = 2,
                CPM = 42,
                Radiation = 0.03,
                Mode = ModeEnum.SLOW
            };
            var expected = "{\"CPS\":2,\"CPM\":42,\"Radiation\":0.03,\"Mode\":\"SLOW\"}";

            // Test
            var result = GeigerValuesConverter.ConvertToJson(value);

            // Verification
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
