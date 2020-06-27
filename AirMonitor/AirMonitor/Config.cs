using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace AirMonitor
{
    class Config
    {
        [JsonProperty("AirlyApiUrl")]
        public static string AirlyApiUrl { get; private set; }
        [JsonProperty("AirlyApiMeasurementUrl")]
        public static string AirlyApiMeasurementUrl { get; private set; }
        [JsonProperty("AirlyApiInstallationUrl")]
        public static string AirlyApiInstallationUrl { get; private set; }
        [JsonProperty("AirlyApiKey")]
        public static string AirlyApiKey { get; private set; }
    }
}
