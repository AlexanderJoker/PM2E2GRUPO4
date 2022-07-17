using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Plugin.AudioRecorder;
using PM2E2GRUPO4.Views;
using PM2E2GRUPO4.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Plugin.Media.Abstractions;

namespace PM2E2GRUPO4
{
    public partial class MainPage : ContentPage
    {
        public String nombre;
        int audio = 0;
        int min, seg;
        bool Grabando;
        bool GraAudio = false;
        byte[] ImagenSave, AudioSave;
        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService()
        {
            StopRecordingAfterTimeout = true,
            TotalAudioTimeout = TimeSpan.FromSeconds(180)
        };
        
        private readonly AudioPlayer audioPlayer = new AudioPlayer();
        public MainPage()
        {
            InitializeComponent();
            obtenerCoordenadas();
            descripcion.Text = "";
            UbiImagen.Source = null;
        }
        
        private async void foto_Clicked(object sender, EventArgs e)
        {
            try
            {
                var TomarFoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Pictures",
                    Name = DateTime.Now.ToString() + "_IMG.jpg",
                    SaveToAlbum = true,
                    CompressionQuality = 50
                });
                
                if (TomarFoto != null)
                {
                    ImagenSave = null;
                    MemoryStream memoryStream = new MemoryStream();

                    TomarFoto.GetStream().CopyTo(memoryStream);
                    ImagenSave = memoryStream.ToArray();

                    UbiImagen.Source = ImageSource.FromStream(() => { return TomarFoto.GetStream(); });
                }

                obtenerCoordenadas();
                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "No Se Puede Tomar Fotografias", "OK");
            }
        }


        public async void obtenerCoordenadas()
        {
            try
            {   
                var localizacion = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10)), new CancellationTokenSource().Token);

                if (localizacion != null)
                {
                    latitud.Text = localizacion.Latitude.ToString();
                    longitud.Text = localizacion.Longitude.ToString();
                }
            }
            catch (FeatureNotSupportedException E01)
            {
                await DisplayAlert("Error", "Dispositivo NO Compatible", "Ok");
            }
            catch (FeatureNotEnabledException E02)
            {
                await DisplayAlert("Error", "Error de Dispositivo", "Ok");
            }
            catch (PermissionException E03)
            {
                await DisplayAlert("Error", "NO Se Aceptaron Permisos de Geolocalizacion ", "Ok");
            }
            catch (Exception E04)
            {
                await DisplayAlert("Error", "No Hay Ubicacion Disponible", "Ok");
            }
        }


        private async void Grabar_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(nombre))
            {
                /////////// 
            }
            else
            {
                if (!audioRecorderService.IsRecording)
                {
                    min = 0;
                    seg = 0;
                    Grabando = true;

                    Device.StartTimer(TimeSpan.FromSeconds(1), () => {
                        seg++;

                        if (seg.ToString().Length == 1)
                        {
                            lblseg.Text = "0" + seg.ToString();
                        }
                        else
                        {
                            lblseg.Text = seg.ToString();
                        }
                        if (seg == 60)
                        {
                            min++;
                            seg = 0;

                            if (min.ToString().Length == 1)
                            {
                                lblmin.Text = "0" + min.ToString();
                            }
                            else
                            {
                                lblmin.Text = min.ToString();
                            }

                            lblseg.Text = "00";
                        }
                        return Grabando;
                    });

                    lblstatus.Text = "Grabando";
                    var grab = await audioRecorderService.StartRecording();
                    Grabar.IsEnabled = false;
                    btnDetener.IsEnabled = true;

                    await grab;

                    GraAudio = true;
                }
            }
        }


        

            private async void btnDetener_Clicked(object sender, EventArgs e)
        {

        }

        private void btnlista_Clicked(object sender, EventArgs e)
        {

        }

        private void btnguardar_Clicked(object sender, EventArgs e)
        {

        }

        private async void btnlista_Clicked(object sender, EventArgs e)
        {
            var nuevapagina = new Listado();
            nuevapagina.Title = "Listado de Lugares";
            await Navigation.PushAsync(nuevapagina);
        }

        private async void btnguardar_Clicked(object sender, EventArgs e)
        {
            
            
                if (String.IsNullOrEmpty(descripcion.Text))
                {
                    await DisplayAlert("Alerta", "Hay Campos Vacios", "Ok");
                }
                else
                {
                    //convertir la imagen a base64
                    string pathBase64Imagen = Convert.ToBase64String(ImagenSave);

                    //extraer el path del audio
                    string audio = AudioPath;
                    //convertir a arreglo de bytes
                    byte[] fileByte = System.IO.File.ReadAllBytes(audio);
                    //convertir el audio a base64
                    string pathBase64Audio = Convert.ToBase64String(fileByte);

                    Sitios save = new Sitios
                    {
                        Id = int.Parse(""),
                        Descripcion = descripcion.Text,
                        Longitud = Convert.ToDouble(longitud.Text),
                        Latitud = Convert.ToDouble(latitud.Text),
                        Foto = pathBase64Imagen,
                        Audio = pathBase64Audio,
                    };

                    Uri RequestUri = new Uri("");

                    var client = new HttpClient();
                    var json = JsonConvert.SerializeObject(save);
                    var contentJson = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(RequestUri, contentJson);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        String jsonx = response.Content.ReadAsStringAsync().Result;

                        JObject jsons = JObject.Parse(jsonx);

                        String Mensaje = jsons["msg"].ToString();

                        await DisplayAlert("Success", "Datos guardados correctamente", "Ok");

                        descripcion.Text = "";
                        ImagenSave = null;
                        AudioSave = null;
                        UbiImagen.Source = null;

                    }
                    else
                    {
                        await DisplayAlert("Error", "Estamos en mantenimiento", "Ok");
                    }

                }

            
            
        }

    

    private async void btnsalir_Clicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

    }
}
