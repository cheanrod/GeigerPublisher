using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GeigerPublisher.Values
{
    public class GeigerValues
    {
        public int CPS { get; set; }
        public int CPM { get; set; }
        public double Radiation { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ModeEnum Mode { get; set; }

        public override string ToString()
        {
            return $"CPS, { CPS }, CPM, { CPM }, uSv/hr, { Radiation }, { Mode.ToString("F") }";
        }
    }
}