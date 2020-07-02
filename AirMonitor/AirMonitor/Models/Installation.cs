using System;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace AirMonitor.Models
{
    public class Installation
    {
        public Installation()
        {

        }

        public Installation(InstallationEntity installationEntity)
        {
            this.Id = installationEntity.Id;
            this.Location = JsonConvert.DeserializeObject<Location>(installationEntity.Location);
            this.Address = JsonConvert.DeserializeObject<Address>(installationEntity.Address);
            this.Elevation = installationEntity.Elevation;
            this.IsAirlyInstallation = installationEntity.IsAirlyInstallation;
        }

        public string Id { get; set; }
        public Location Location { get; set; }
        public Address Address { get; set; }
        public double Elevation { get; set; }
        [JsonProperty(PropertyName = "airly")]
        public bool IsAirlyInstallation { get; set; }
    }
}
