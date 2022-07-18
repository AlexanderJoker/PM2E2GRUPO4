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
        }

        private void ListaSitios_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }
    }
}