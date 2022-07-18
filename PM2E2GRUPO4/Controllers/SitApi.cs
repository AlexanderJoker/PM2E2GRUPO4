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
                using (HttpClient cliente = new HttpClient())
            {
                var respuesta = await cliente.GetAsync("https://activaciones3-02.000webhostapp.com/api/getLugares.php");

                if (respuesta.IsSuccessStatusCode)
                {
                    string contenido = respuesta.Content.ReadAsStringAsync().Result.ToString();

                    dynamic dyn = JsonConvert.DeserializeObject(contenido);
                    byte[] newBytes = null;
                    if (contenido.Length > 28)
                    {

                        foreach (var item in dyn.LISTAD0)
                        {
                            string img64 = item.foto.ToString();
                            newBytes = Convert.FromBase64String(img64);
                            var stream = new MemoryStream(newBytes);

                            string audio64 = item.audio.ToString();
                            byte[] decodedString = Base64.Decode(audio64, Base64Flags.Default);

                            listasitios.Add(new SitiosListado(
                                            item.id.ToString(), 
                                            item.descripcion.ToString(),
                                            item.latitud.ToString(), 
                                            item.longitud.ToString(),
                                            ImageSource.FromStream(() => stream),
                                            audio64, 
                                            decodedString
                                            ));
                        }
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