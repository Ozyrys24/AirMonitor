using Newtonsoft.Json;
using SQLite;
using System;
namespace AirMonitor.Models
{
    public class MeasurementItemEntity
    {
        public MeasurementItemEntity()
        {

        }

        public MeasurementItemEntity(MeasurementItem current)
        {
            this.FromDateTime = current.FromDateTime;
            this.TillDateTime = current.TillDateTime;
            this.Values = JsonConvert.SerializeObject(current.Values);
            this.Indexes = JsonConvert.SerializeObject(current.Indexes);
            this.Standards = JsonConvert.SerializeObject(current.Standards);
        }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime FromDateTime { get; set; }
        public DateTime TillDateTime { get; set; }
        public string Values { get; set; }
        public string Indexes { get; set; }
        public string Standards { get; set; }
    }
}
