using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq;
using System.Transactions;

namespace AirMonitor.Models
{
    public class Measurement
    {
        public Measurement()
        {
                
        }

        public Measurement(MeasurementEntity measurement)
        {
            this.CurrentDisplayValue = measurement.CurrentDisplayValue;
            this.History = JsonConvert.DeserializeObject<MeasurementItem[]>(measurement.History);
            this.Forecast = JsonConvert.DeserializeObject<MeasurementItem[]>(measurement.Forecast);
        }

        public int CurrentDisplayValue { get; set; }
        public MeasurementItem Current { get; set; }
        public MeasurementItem[] History { get; set; }
        public MeasurementItem[] Forecast { get; set; }
        public Installation Installation { get; set; }
    }
}
