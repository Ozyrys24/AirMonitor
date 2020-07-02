using AirMonitor.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Data;

namespace AirMonitor
{
    public class DatabaseHelper
    {
        private SQLiteAsyncConnection dbContext = new SQLiteAsyncConnection(Path.Combine(
                 Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                 "Database.db3"), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
        public SQLiteAsyncConnection DbContext { get => dbContext; set => dbContext = value; }

        public SQLiteConnectionWithLock DatabasebConnection { get; set; }

        public DatabaseHelper()
        {
           // DatabasebConnection = DbContext.GetConnection();
            //CreateTables();
        }

        public async void CreateTables()
        {
            await dbContext.CreateTableAsync<AirQualityIndex>();
            await dbContext.CreateTableAsync<AirQualityStandard>();
            await dbContext.CreateTableAsync<InstallationEntity>();
            await dbContext.CreateTableAsync<MeasurementEntity>();
            await dbContext.CreateTableAsync<MeasurementItemEntity>();
            await dbContext.CreateTableAsync<MeasurementValue>();
            await dbContext.CreateTableAsync<TillTime>();
        }

        public async void InstallationSave(IEnumerable<Installation> installationList)
        {
            await dbContext.DeleteAllAsync<InstallationEntity>();

            foreach (var installation in installationList)
                await dbContext.InsertAsync(new InstallationEntity(installation));
        }

        public List<Installation> GetInstallation()
        {
            var installationList = new List<Installation>();
            foreach (var installationEntity in dbContext.Table<InstallationEntity>().ToListAsync().Result)
                installationList.Add(new Installation(installationEntity));
            return installationList;   
        }

        public async void SetTime()
        {
            await dbContext.DeleteAllAsync<TillTime>();
            await dbContext.InsertAsync(new TillTime()
            {
                Time = DateTime.Now
            });
        }

        public DateTime GetTime()
        {
            var tillTime = DbContext.Table<TillTime>().FirstOrDefaultAsync().Result;
            if (tillTime == null) tillTime = new TillTime() { Time = new DateTime(1, 1, 1, 1, 1, 1) };
            return tillTime.Time;
        }

        public List<Measurement> GetMeasurement()
        {
            var measurementList = new List<Measurement>();
            foreach (var measurementEntity in dbContext.Table<MeasurementEntity>().ToListAsync().Result)
            {
                measurementList.Add(new Measurement(measurementEntity)
                {
                    Current = new MeasurementItem(dbContext.Table<MeasurementItemEntity>().ElementAtAsync(measurementEntity.Id).Result)
                    {
                        Standards = dbContext.Table<AirQualityStandard>().Where(x => x.Id == measurementEntity.Id).ToArrayAsync().Result,
                        Indexes = dbContext.Table<AirQualityIndex>().Where(x => x.Id == measurementEntity.Id).ToArrayAsync().Result,
                        Values = dbContext.Table<MeasurementValue>().Where(x => x.Id == measurementEntity.Id).ToArrayAsync().Result
                    },
                    Installation = new Installation(dbContext.GetAsync<InstallationEntity>(measurementEntity.Installation).Result)
                });
            }
            return measurementList;
        }

        public async void MeasurementSave(IEnumerable<Measurement> measurementList)
        {
            await dbContext.DeleteAllAsync<AirQualityIndex>();
            await dbContext.DeleteAllAsync<AirQualityStandard>();
            await dbContext.DeleteAllAsync<MeasurementEntity>();
            await dbContext.DeleteAllAsync<MeasurementItemEntity>();
            await dbContext.DeleteAllAsync<MeasurementValue>();

            foreach (var measurement in measurementList)
            {
                await dbContext.InsertAllAsync(measurement.Current.Standards);
                await dbContext.InsertAllAsync(measurement.Current.Indexes);
                await dbContext.InsertAllAsync(measurement.Current.Values);
                await dbContext.InsertAsync(new MeasurementEntity(measurement));
                await dbContext.InsertAsync(new MeasurementItemEntity(measurement.Current));
            }

 
        }
    }
}
