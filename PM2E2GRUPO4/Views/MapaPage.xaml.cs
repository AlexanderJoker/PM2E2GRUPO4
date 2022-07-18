using Plugin.Geolocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace PM2E2GRUPO4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapaPage : ContentPage
    {
        public MapaPage()
        {
            InitializeComponent();
        }

        private async void btnRuta_Clicked(object sender, EventArgs e)
        {
            
        }


        private void Localizacion_positionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            int value1 = 1; int value2 = 1;
            Position movetomaps = new Position(Convert.ToDouble(lbllat.Text), Convert.ToDouble(lbllong.Text));
            mapitaa.MoveToRegion(new MapSpan(movetomaps, value1, value2));
        }
        protected async override void OnAppearing()
        {
           base.OnAppearing();
            double Latitud = Convert.ToDouble(lbllat.Text);
            double Longitud = Convert.ToDouble(lbllong.Text);
            //COLOCACION DE PIN EN EL MAPA
            Pin ubicacion = new Pin();
            ubicacion.Label = ""+ lbldesc.Text;
            ubicacion.Address = ""+ Latitud+","+ Longitud;
            ubicacion.Type = PinType.Generic;
            ubicacion.Position = new Position(Latitud, Longitud);
            mapitaa.Pins.Add(ubicacion);

            //MOVERSE A LA REGION DE LA LOCALIZACION OBTENIDA
            var location = await Geolocation.GetLocationAsync();
            if (location == null) { location = await Geolocation.GetLastKnownLocationAsync(); }
            //mapitaa.MoveToRegion(new MapSpan(new Position(Latitud, Longitud), 0.05, 0.05));
            mapitaa.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(Latitud, Longitud), Distance.FromMeters(100)));

            var localizacion = CrossGeolocator.Current;

            if(localizacion != null)
            {
                localizacion.PositionChanged += Localizacion_positionChanged;

                if (!localizacion.IsListening)
                {
                    await localizacion.StartListeningAsync(TimeSpan.FromSeconds(10), 100);
                }
            }
        }

        private async void btnDriving_Clicked(object sender, EventArgs e)
        {
            Double txtlat = Convert.ToDouble(lbllat.Text);
            Double txtlogn = Convert.ToDouble(lbllong.Text);
            var geolocator = new Location(txtlat, txtlogn);
            await Xamarin.Essentials.Map.OpenAsync(geolocator, new MapLaunchOptions { NavigationMode = NavigationMode.Driving });
        }
    }
}