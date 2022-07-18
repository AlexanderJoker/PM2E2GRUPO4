using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Plugin.AudioRecorder;
using Plugin.Media;
using PM2E2GRUPO4.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PM2E2GRUPO4.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Update_Page : ContentPage
    {
        byte[] ImagenSave;
        bool GraAudio = false;
        public String nombre;
        public bool tomefoto = false;
        string audio, foto;
        int min, seg;
        bool Grabando;
        private readonly AudioRecorderService audioRecorderService = new AudioRecorderService()
        {
            StopRecordingAfterTimeout = true,
            TotalAudioTimeout = TimeSpan.FromSeconds(180)
        };
        public Update_Page(String audio,String foto)
        {
            InitializeComponent();
            this.audio= audio;
            this.foto= foto;
            nombre = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DateTime.Now.ToString("ddMMyyyymmss").Trim() + "Audio_.wav");
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            //metodo de tap de la imagen
            try
            {
                var TomarFoto = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Directory = "Pictures",
                    Name = DateTime.Now.ToString() + "_IMG.jpg",
                    SaveToAlbum = true,
                    CompressionQuality = 40
                });

                if (TomarFoto != null)
                {
                    ImagenSave = null;
                    MemoryStream memoryStream = new MemoryStream();

                    TomarFoto.GetStream().CopyTo(memoryStream);
                    ImagenSave = memoryStream.ToArray();
                    tomefoto = true;
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
                    lat.Text = localizacion.Latitude.ToString();
                    longg.Text = localizacion.Longitude.ToString();
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

            }
            else
            {
                if (!audioRecorderService.IsRecording)
                {
                    min = 0;
                    seg = 0;
                    Grabando = true;

                    Device.StartTimer(TimeSpan.FromSeconds(1), () =>
                    {
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

                    lblstatu.Text = "Grabando";
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
            Grabar.IsEnabled = true;
            btnDetener.IsEnabled = false;

            Grabando = false;
            lblmin.Text = "00";
            lblseg.Text = "00";
            lblstatu.Text = "Grabacion Terminada";

            desc.IsEnabled = true;
            btnedit.IsEnabled = true;

            await audioRecorderService.StopRecording();
            var stream = audioRecorderService.GetAudioFileStream();
            bool NoExist = File.Exists(nombre);
            if (!NoExist)
            {
                /////////Audio Existente por Defecto 
            }

            else
            {
                String[] FRecord = nombre.Split('/');
                int tamfile = FRecord.Length;
                String nombreA = FRecord[tamfile - 1];
                String valorResultante = new String(nombreA.Where(Char.IsDigit).ToArray());

                if (String.IsNullOrEmpty(valorResultante))
                {
                    /////// Usar el mismo archivo ////// 
                }
                else
                {
                    int num = Int32.Parse(valorResultante);
                    nombre = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DateTime.Now.ToString("ddMMyyyymmss").Trim() + "Audio_.wav");
                }
            }

            using (var audiofile = new FileStream(nombre, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(audiofile);
            }
            await DisplayAlert("AVISO", "Audio Guardado", "Aceptar");
        }

        private void desc_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnedit.IsEnabled = true;
        }
        private async void btnedit_Clicked(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(desc.Text) || desc.Text.Length > 100)
            {
                await DisplayAlert("Alerta", "Hay Campos Vacios o descripcion muy larga", "Ok");
            }
            else
            {
                if (audio!=null && foto!=null)//solo editar si hay audio y tomo foto
                {
                    string pathBase64Imagen = foto;
                    //byte[] fileByte = System.IO.File.ReadAllBytes(audio);
                    string pathBase64Audio = audio;
                    Sitios save = new Sitios
                    {
                        Id = Int32.Parse(txtid.Text),
                        Descripcion = desc.Text,
                        Latitud = Convert.ToDouble(lat.Text),
                        Longitud = Convert.ToDouble(longg.Text),
                        Foto = pathBase64Imagen,
                        Audio = pathBase64Audio,
                    };

                    Uri RequestUri = new Uri("https://activaciones3-02.000webhostapp.com/api/updateLugar.php");

                    var client = new HttpClient();
                    client.BaseAddress = new Uri("https://activaciones3-02.000webhostapp.com");
                    var json = JsonConvert.SerializeObject(save);
                    var contenidoJson = new StringContent(json, Encoding.UTF8, "application/json");
                    var respuestajson = await client.PostAsync("/api/updateLugar.php", contenidoJson);
                    var result = await respuestajson.Content.ReadAsStringAsync();

                    if (respuestajson.StatusCode == HttpStatusCode.OK)
                    {
                        //JObject respuestastojson = JObject.Parse(respuestajson.Content.ReadAsStringAsync().Result);

                        await DisplayAlert("success", "Datos Modificados correctamente!", "ok");

                        desc.Text = "";
                        ImagenSave = null;
                        //AudioSave = null;
                        UbiImagen.Source = null;
                        lblmin.Text = "00";
                        lblseg.Text = "00";
                        tomefoto = false;
                        lblstatu.Text = "";
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("error", "no hay acceso a la pagina web", "ok");
                    }
                }
                else
                {
                    await DisplayAlert("Alerta", "El audio no puede quedar vacio y la foto si modificar", "Ok");
                }
            }
        }
    }
}