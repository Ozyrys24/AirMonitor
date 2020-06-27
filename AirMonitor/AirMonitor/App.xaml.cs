using AirMonitor.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace AirMonitor
{
    public partial class App : Application
    {


        public App()
        {
            GetJSON();
            InitializeComponent();
            
            MainPage = new RootTabbedPage();

        }

        private void GetJSON()
        {
            var json = new StreamReader((Assembly.GetAssembly(typeof(App))).GetManifestResourceStream("AirMonitor.Config.json"));
            _ = JsonConvert.DeserializeObject<Config>(json.ReadToEnd());
        }
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
