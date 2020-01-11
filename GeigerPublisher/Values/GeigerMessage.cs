using System;
using Newtonsoft.Json;

namespace GeigerPublisher.Values
{
    public class GeigerMessage
    {
        [JsonProperty("ts")]
        [JsonConverter(typeof(UnixMillisecondsConverter))]
        public DateTime Timestamp { get; set; }
        [JsonProperty("val")]
        [JsonConverter(typeof(RoundingJsonConverter), 3)]
        public double Radiation { get; set; }
    }
}