﻿using AirMonitor.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace AirMonitor.ViewModels
{
    class DetailsViewModel : BaseViewModel
    {
        public DetailsViewModel()
        {
        }

        private Measurement _item;
        public Measurement Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);

                UpdateProperties();
            }
        }


        private void UpdateProperties()
        {
            if (Item?.Current == null) return;
            var current = Item?.Current;
            var index = current.Indexes?.FirstOrDefault(c => c.Name == "AIRLY_CAQI");
            var values = current.Values;
            var standards = current.Standards;

            CaqiValue = (int)Math.Round(index?.Value ?? 0);
            CaqiTitle = index.Description;
            CaqiDescription = index.Advice;
            Pm25Value = (int)Math.Round(values?.FirstOrDefault(s => s.Name == "PM25")?.Value ?? 0);
            Pm10Value = (int)Math.Round(values?.FirstOrDefault(s => s.Name == "PM10")?.Value ?? 0);
            HumidityValue = (int)Math.Round(values?.FirstOrDefault(s => s.Name == "HUMIDITY")?.Value ?? 0);
            PressureValue = (int)Math.Round(values?.FirstOrDefault(s => s.Name == "PRESSURE")?.Value ?? 0);
            Pm25Percent = (int)Math.Round(standards?.FirstOrDefault(s => s.Pollutant == "PM25")?.Percent ?? 0);
            Pm10Percent = (int)Math.Round(standards?.FirstOrDefault(s => s.Pollutant == "PM10")?.Percent ?? 0);
        }

        private int _caqiValue;
        public int CaqiValue
        {
            get => _caqiValue;
            set => SetProperty(ref _caqiValue, value);
        }

        private string _caqiTitle = "Świetna jakość!";
        public string CaqiTitle
        {
            get => _caqiTitle;
            set => SetProperty(ref _caqiTitle, value);
        }

        private string _caqiDescription = "Możesz bezpiecznie wyjść z domu bez swojej maski anty-smogowej i nie bać się o swoje zdrowie.";
        public string CaqiDescription
        {
            get => _caqiDescription;
            set => SetProperty(ref _caqiDescription, value);
        }

        private int _pm25Value = 34;
        public int Pm25Value
        {
            get => _pm25Value;
            set => SetProperty(ref _pm25Value, value);
        }

        private int _pm25Percent = 137;
        public int Pm25Percent
        {
            get => _pm25Percent;
            set => SetProperty(ref _pm25Percent, value);
        }

        private int _pm10Value = 67;
        public int Pm10Value
        {
            get => _pm10Value;
            set => SetProperty(ref _pm10Value, value);
        }

        private int _pm10Percent = 135;
        public int Pm10Percent
        {
            get => _pm10Percent;
            set => SetProperty(ref _pm10Percent, value);
        }

        private double _humidityValue = 0.95;
        public double HumidityValue
        {
            get => _humidityValue;
            set => SetProperty(ref _humidityValue, value);
        }

        private int _pressureValue = 1027;
        public int PressureValue
        {
            get => _pressureValue;
            set => SetProperty(ref _pressureValue, value);
        }

    }

}
