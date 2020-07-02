using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace AirMonitor.Models
{
    public class InstallationEntity
    {
        public InstallationEntity()
        {

        }

        public InstallationEntity(Installation installation)
        {
            this.Id = installation.Id;
            this.Location = JsonConvert.SerializeObject(installation.Location);
            this.Address = JsonConvert.SerializeObject(installation.Address);
            this.Elevation = installation.Elevation;
            this.IsAirlyInstallation = installation.IsAirlyInstallation;
        }

        [PrimaryKey]
        public string Id { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public double Elevation { get; set; }
        [JsonProperty(PropertyName = "airly")]
        public bool IsAirlyInstallation { get; set; }
    }
}
