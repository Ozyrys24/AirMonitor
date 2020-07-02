using AirMonitor.Models;
using AirMonitor.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Web;
using System.Globalization;

namespace AirMonitor.ViewModels
{
    internal class HomeViewModel : BaseViewModel
    {
        private readonly HttpClient Client = new HttpClient();
        private IEnumerable<Installation> _installations;
        private IEnumerable<Measurement> data;
        private Location location;
        private DateTime tillTime;

        private readonly INavigation _navigation;

        public IEnumerable<Installation> Installations { get; set; }
        public HomeViewModel(INavigation navigation)
        {
            _navigation = navigation;
            ClientInit();
            _ = Initialize();
        }

        private ICommand _goToDetailsCommand;
        public ICommand GoToDetailsCommand => _goToDetailsCommand ?? (_goToDetailsCommand = new Command<Measurement>(OnGoToDetails));

        private void OnGoToDetails(Measurement item)
        {
            _navigation.PushAsync(new DetailsPage(item));
        }

        private List<Measurement> _items;
        public List<Measurement> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }


        public void ClientInit()
        {
            Client.BaseAddress = new Uri(Config.AirlyApiUrl);
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            Client.DefaultRequestHeaders.Add("apikey", Config.AirlyApiKey);
        }

        private async Task Initialize()
        {
            IsBusy = true;

            //tillTime = App.DbHelper.GetTime();
           // bool test = (location != await GetLocation() && (tillTime < DateTime.Now.Subtract(new TimeSpan(1, 0, 0)) || tillTime > DateTime.Now));
           // if (location != await GetLocation() && (tillTime < DateTime.Now.Subtract(new TimeSpan(1,0,0)) || tillTime > DateTime.Now))
            //{
                location = await GetLocation();
                Installations = await GetInstallations(location, maxResults: 3);
                data = await GetMeasurementsForInstallations(Installations);
            //} 
            //else
            //{
            //    installations = App.DbHelper.GetInstallation();
            //    data = App.DbHelper.GetMeasurement();
            //}
            Items = new List<Measurement>(data);
            //App.DbHelper.InstallationSave(installations);
            //App.DbHelper.MeasurementSave(data);
                
            IsBusy = false;
        }

        private async Task<IEnumerable<Installation>> GetInstallations(Location location, double maxDistanceInKm = 3, int maxResults = -1)
        {
            if (location == null)
            {
                System.Diagnostics.Debug.WriteLine("No location data.");
                return null;
            }

           // App.DbHelper.SetTime();

            string query = GetQuery(new Dictionary<string, object>
            {
                { "lat", location.Latitude },
                { "lng", location.Longitude },
                { "maxDistanceKM", maxDistanceInKm },
                { "maxResults", maxResults }
            });
            string url = GetAirlyApiUrl(Config.AirlyApiInstallationUrl, query);

            IEnumerable<Installation> response = await GetHttpResponseAsync<IEnumerable<Installation>>(url);
            return response;
        }

        private async Task<IEnumerable<Measurement>> GetMeasurementsForInstallations(IEnumerable<Installation> installations)
        {
            if (installations == null)
            {
                System.Diagnostics.Debug.WriteLine("No installations data.");
                return null;
            }

            var measurements = new List<Measurement>();

            foreach (var installation in installations)
            {
                var query = GetQuery(new Dictionary<string, object>
                {
                    { "installationId", installation.Id }
                });
                var url = GetAirlyApiUrl(Config.AirlyApiMeasurementUrl, query);

                var response = await GetHttpResponseAsync<Measurement>(url);

                if (response != null)
                {
                    response.Installation = installation;
                    response.CurrentDisplayValue = (int)Math.Round(response.Current?.Indexes?.FirstOrDefault()?.Value ?? 0);
                    measurements.Add(response);
                }
            }

            return measurements;
        }

        private async Task<T> GetHttpResponseAsync<T>(string url)
        {
            try
            {
                var response = await Client.GetAsync(url);

                if (response.Headers.TryGetValues("X-RateLimit-Limit-day", out var dayLimit) &&
                    response.Headers.TryGetValues("X-RateLimit-Remaining-day", out var dayLimitRemaining))
                {
                    System.Diagnostics.Debug.WriteLine($"Day limit: {dayLimit?.FirstOrDefault()}, remaining: {dayLimitRemaining?.FirstOrDefault()}");
                }

                switch ((int)response.StatusCode)
                {
                    case 200:
                        var content = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(content);
                        return result;
                    case 429: // too many requests
                        System.Diagnostics.Debug.WriteLine("Too many requests");
                        break;
                    default:
                        var errorContent = await response.Content.ReadAsStringAsync();
                        System.Diagnostics.Debug.WriteLine($"Response error: {errorContent}");
                        return default;
                }
            }
            catch (JsonReaderException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }

            return default;
        }

        private string GetAirlyApiUrl(string path, string query)
        {
            UriBuilder builder = new UriBuilder(Config.AirlyApiUrl)
            {
                Port = -1
            };
            builder.Path += path;
            builder.Query = query;
            string url = builder.ToString();

            return url;
        }

        private string GetQuery(IDictionary<string, object> args)
        {
            if (args == null)
            {
                return null;
            }

            var query = HttpUtility.ParseQueryString(string.Empty);

            foreach (KeyValuePair<string, object> arg in args)
            {
                if (arg.Value is double number)
                {
                    query[arg.Key] = number.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    query[arg.Key] = arg.Value?.ToString();
                }
            }

            return query.ToString();
        }

        public async Task<Location> GetLocation()
        {
            try
            {
                return await Geolocation.GetLastKnownLocationAsync()
                ?? await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));
            }
            catch (FeatureNotSupportedException e)
            {
                Debug.Print(e.Message);
            }
            catch (FeatureNotEnabledException e)
            {
                Debug.Print(e.Message);
            }
            catch (PermissionException e)
            {
                Debug.Print(e.Message);
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

            return null;
        }

    }
}
