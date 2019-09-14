using System;
using System.Globalization;
using Newtonsoft.Json;

namespace GeigerPublisher.Values
{
    public static class GeigerValuesConverter
    {
        public static GeigerValues ConvertFromReading(string reading)
        {
            CultureInfo culture = new CultureInfo("en-US");
            var geigerValues = new GeigerValues();

            var values = reading.Split(",");
            geigerValues.CPS = int.Parse(values[1], culture);
            geigerValues.CPM = int.Parse(values[3], culture);
            geigerValues.Radiation = double.Parse(values[5], culture);
            switch (values[6].Trim())
            {
                case "FAST":
                    geigerValues.Mode = ModeEnum.FAST;
                    break;
                case "SLOW":
                    geigerValues.Mode = ModeEnum.SLOW;
                    break;
                case "INST":
                    geigerValues.Mode = ModeEnum.INST;
                    break;
                default:
                    throw new FormatException("Cannot convert mode value: " + values[6].Trim());
            }
            return geigerValues;
        }

        public static string ConvertToJson(GeigerValues geigerValues)
        {
            return JsonConvert.SerializeObject(geigerValues);
        }
    }
}