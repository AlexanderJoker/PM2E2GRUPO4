using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PM2E2GRUPO4.Models;
using PM2E2GRUPO4.Controllers;

using Xamarin.Essentials;
using System.Net.Http;

using Newtonsoft.Json;
using System.IO;
//using Android.Util;
using Plugin.AudioRecorder;
using MediaManager;
using Android.Util;
//using Android.Media;
//using Stream = Android.Media.Stream;



namespace PM2E2GRUPO4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Listado : ContentPage
    {
        private readonly AudioPlayer audioPlayer = new AudioPlayer();

        //private readonly MediaPlayer mediaPlayer = new MediaPlayer();

        string txtDescripcionSeleccionada;
        double dbLatitud, dbLongitud;

        List<SitiosListado> lista = new List<SitiosListado>();

        Object objSitioGlobal = null;
        string idGlobal = "";
        string sitioGlobal = "";
        string latitud = "";
        string longitud = "";

        string audio64 = null;
        byte[] decodedString = null;


        public Listado()
        {
            InitializeComponent();
            lista.Clear();
            GetSitiosList();
        }

        private void btnDelete_Clicked(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Clicked(object sender, EventArgs e)
        {

        }

        private void btnaudio_Clicked(object sender, EventArgs e)
        {

        }

        private void btnVerMapa_Clicked(object sender, EventArgs e)
        {

        }



        private async void GetSitiosList()
        {
            var AccesoInternet = Connectivity.NetworkAccess;

            if (AccesoInternet == NetworkAccess.Internet)
            {
                //sl.IsVisible = true;


                List<SitiosListado> listapersonas = new List<SitiosListado>();
                listapersonas = await SitApi.ControllerObtenerListaSitios();

                if (listapersonas.Count > 0)
                {
                    ListaSitios.ItemsSource = null;
                    ListaSitios.ItemsSource = listapersonas;
                }
                else
                {
                    await DisplayAlert("Notificación", $"Lista vacía, ingrese datos", "Ok");
                }

                //sl.IsVisible = false;

            }
        }

        private async void ListaSitios_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            string action = await DisplayActionSheet("Elige La Accion que desea realizar", "Cancelar", null, "Eliminar", "Modificar", "Ir al Mapa", "Reproducir Audio");
            if (action.Contains("Reproducir Audio"))
            {
                var rep = e.Item as SitiosListado;
                string rep2 = rep.Audio;
                byte[] rep3 = Base64.Decode(rep2, Base64Flags.Default);

                await CrossMediaManager.Current.Play(rep3);
            }

            if (action.Contains("Ir al Mapa"))
            {
                var rep = e.Item as SitiosListado;
                Ubicacion ubi = new Ubicacion
                {
                    latitud = Convert.ToDouble(rep.Latitud),
                    longitud = Convert.ToDouble(rep.Longitud),
                };

                MapaPage mapita = new MapaPage();
                mapita.BindingContext = ubi;
                await Navigation.PushAsync(mapita);
            }

            if (action.Contains("Modificar"))
            {
                var rep = e.Item as SitiosListado;
                Object sit = new
                {
                    id = rep.Id,
                    latitud = Convert.ToDouble(rep.Latitud),
                    longitud = Convert.ToDouble(rep.Longitud),
                    descripcion = rep.Descripcion,
                    imagen = ImageSource.FromStream(() => new MemoryStream(Convert.FromBase64String(rep.Foto2))),
                    audio = decodedString
                };

                Update_Page up = new Update_Page();
                up.BindingContext = sit;
                await Navigation.PushAsync(up);
            }

            if (action.Contains("Eliminar"))
            {
                bool res = await DisplayAlert("Notificación" , "¿Esta seguro de eliminar el sitio?", "Sí", "Cancelar");
                if (res)
                {
                    var rep = e.Item as SitiosListado;

                    Uri RequestUri = new Uri("http://activaciones3-02.000webhostapp.com/api/deleteLugar.php");
                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(rep.Id);

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Post,
                        RequestUri = RequestUri
                    };

                    HttpResponseMessage response = await client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Notificación", "Registro eliminado con éxito", "Ok");
                        lista.Clear();
                        GetSitiosList();
                    }
                    else
                    {
                        await DisplayAlert("Notificación", "Ha ocurrido un error", "Ok");
                    }
                }
            }
        }


    }
}
