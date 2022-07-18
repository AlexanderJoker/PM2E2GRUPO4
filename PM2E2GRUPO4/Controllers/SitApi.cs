using System;
using System.Collections.Generic;
using System.Text;
using PM2E2GRUPO4.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using Xamarin.Forms;
using Android.Util;
using Microsoft.CSharp;

namespace PM2E2GRUPO4.Controllers
{
    public class SitApi
    {
        public async static Task<List<SitiosListado>> ControllerObtenerListaSitios()
        {
            List<SitiosListado> listasitios = new List<SitiosListado>();
            try
            {
                using (HttpClient requestHTTP = new HttpClient())
                {
                var repomse = await requestHTTP.GetAsync("https://activaciones3-02.000webhostapp.com/api/getLugares.php");

                if (repomse.IsSuccessStatusCode)
                {
                    string content = repomse.Content.ReadAsStringAsync().Result.ToString();

                    dynamic vardinamic = JsonConvert.DeserializeObject(content);
                    byte[] imgBytes = null;

                    foreach (var obj in vardinamic.LISTAD0)
                    {
                            //OBTIENE LA FOTO EN JSON BASE64 Y LA PASA A STREAM
                        string imgEncript = obj.foto.ToString();
                        imgBytes = Convert.FromBase64String(imgEncript);
                        var stream = new MemoryStream(imgBytes);
                            //OBTENER EL AUDIO Y SACARLO DE BASE 64
                        string audioencript = obj.audio.ToString();
                        byte[] audioBytes = Base64.Decode(audioencript, Base64Flags.Default);

                        listasitios.Add(new SitiosListado(
                                        obj.id.ToString(), 
                                        obj.descripcion.ToString(),
                                        obj.latitud.ToString(), 
                                        obj.longitud.ToString(),
                                        ImageSource.FromStream(() => stream),
                                        imgEncript,
                                        audioencript, 
                                        audioBytes
                                        ));
                    }
                    
                }
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.Message + "\n" + ex.StackTrace);
            }
            return listasitios;
        }



    }
}