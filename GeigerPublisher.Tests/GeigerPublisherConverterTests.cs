using System;
using NUnit.Framework;
using GeigerPublisher.Values;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

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

        [Test]
        public void ConvertJSONMessage()
        {
            CultureInfo culture = new CultureInfo("en-US");
            var now = DateTime.Now;
            var timeSpan = now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timestamp = (long)timeSpan.TotalMilliseconds;
            var values = new List<double>();
            for (int i = 0; i < 10; i++)
            {
                values.Add(i);
            }
            var average = values.Average();
            
            var expected = $"{{\"ts\":{timestamp},\"val\":{average.ToString(culture)}}}";

            // Test
            var result = GeigerValuesConverter.ConvertToJson(now, values);

            // Verification
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
