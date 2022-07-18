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
using Android.Media;
//using Android.Media;
//using Stream = Android.Media.Stream;



namespace PM2E2GRUPO4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Listado : ContentPage
    {
        private readonly AudioPlayer audioPlayer = new AudioPlayer();
        protected MediaPlayer player;
        AudioPlayer player2;
        //private readonly MediaPlayer mediaPlayer = new MediaPlayer();

        List<SitiosListado> lista = new List<SitiosListado>();

   
        byte[] decodedString = null;


        public Listado()
        {
            InitializeComponent();
            lista.Clear();
            GetSitiosList();
            player2 = new AudioPlayer();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            lista.Clear();
            GetSitiosList();
        }
        private async void GetSitiosList()
        {//LLENA LA LISTA
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                List<SitiosListado> listLugares = new List<SitiosListado>();
                listLugares = await SitApi.ControllerObtenerListaSitios();

                if (listLugares.Count >= 1)
                {
                    ListaSitios.ItemsSource = null;
                    ListaSitios.ItemsSource = listLugares;
                }
                else
                {
                    await DisplayAlert("INFO", "Aun no hay Lugares por aki", "OK");
                }
            }
        }

        private async void ListaSitios_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var rep = e.Item as SitiosListado;
            string action = await DisplayActionSheet("Elige La Accion que desea realizar", "Cancelar", null, "Eliminar", "Modificar", "Ir al Mapa", "Reproducir Audio");
            if (rep.Audio != null)
            {
                if (action.Contains("Reproducir Audio"))
                {
                    //SACAR EL AUDIO DE EL MODELO DE SITIOS
                    string rep2 = rep.Audio;
                    byte[] rep3 = Base64.Decode(rep2, Base64Flags.Default);
                    //CONVERSION Y CREACION DE ARCHIVO DE AUDIO TEMPORAL
                    string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sample.wav");
                    System.IO.Stream stream = new MemoryStream(rep3);
                    using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write)) { stream.CopyTo(fileStream); }
                    //await CrossMediaManager.Current.Play(fileName);
                    player2.Play(fileName);
                }
            }
            else {
                await DisplayAlert("ALERTA","AKI NO HAY AUDIOS","OK");
            }
            if (action.Contains("Cancelar")) 
            {

            }
            if (action.Contains(""))
            {

            }
            if (action.Contains("Ir al Mapa"))
            {
                Ubicacion ubi = new Ubicacion
                {
                    latitud = Convert.ToDouble(rep.Latitud),
                    longitud = Convert.ToDouble(rep.Longitud),
                    descripcion = rep.Descripcion
                };

                MapaPage mapita = new MapaPage();
                mapita.BindingContext = ubi;
                mapita.Title="Lat="+ubi.latitud + ",Long=" + ubi.longitud;
                await Navigation.PushAsync(mapita);
            }

            if (action.Contains("Modificar"))
            {
                //OBJETO TEMPORAL PARA ENVIARLO A BINDING DE EDITAR
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
                up.Title = "ACTUALIZANDO ID: " + rep.Id;
                await Navigation.PushAsync(up);
            }

            if (action.Contains("Eliminar"))
            {
                bool r = await DisplayAlert("ALERTA" , "¿Esta seguro de eliminar el sitio?", "Sí", "Cancelar");
                if (r)
                {
                    Object obb = new { id = rep.Id };
                    var json = JsonConvert.SerializeObject(obb);
                    var request = new HttpRequestMessage
                    {
                        Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
                        Method = HttpMethod.Post,
                        RequestUri = new Uri("http://activaciones3-02.000webhostapp.com/api/deleteLugar.php")
                    };

                    //SI RESPONDE CON UN CODIGO 200 SE ELIMINO
                    var response = await new HttpClient().SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        lista.Clear();
                        GetSitiosList();
                        await DisplayAlert("INFO", "Registro eliminado con éxito", "Ok");
                    }
                    else
                    {
                        await DisplayAlert("ERROR", "Ha ocurrido un error", "Ok");
                    }
                }
            }
        }


    }
}
