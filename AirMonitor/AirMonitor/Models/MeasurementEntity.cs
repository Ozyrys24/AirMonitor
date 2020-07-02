using Newtonsoft.Json;
using SQLite;

namespace AirMonitor.Models
{
    public class MeasurementEntity
    {
        public MeasurementEntity()
        {

        }
        public MeasurementEntity(Measurement measurement)
        {
            this.CurrentDisplayValue = measurement.CurrentDisplayValue;
            this.Current = measurement.Current.Id;
            this.History = JsonConvert.SerializeObject(measurement.History);
            this.Forecast = JsonConvert.SerializeObject(measurement.Forecast);
            if (!int.TryParse(measurement.Installation.Id, out var installation)) installation = 0;
            this.Installation = installation;
        }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CurrentDisplayValue { get; set; }
        public int Current { get; set; }
        public string History { get; set; }
        public string Forecast { get; set; }
        public int Installation { get; set; }
    }
}
