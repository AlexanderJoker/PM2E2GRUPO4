using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace PM2E2GRUPO4.Models
{
    public class SitiosListado
    {
        public SitiosListado(string Id, string Descripcion, string Latitud, string Longitud, ImageSource Foto, string Audio, byte[] decodedStringAudio)
        {
            this.Id = Id;
            this.Descripcion = Descripcion;
            this.Latitud = Latitud;
            this.Longitud = Longitud;
            this.Foto = Foto;
            this.Audio = Audio;
            this.decodedStringAudio = decodedStringAudio;
        }

        public string Id { get; set; }
        public string Descripcion { get; set; }
        public string Latitud { get; set; }
        public string Longitud { get; set; }
        public ImageSource Foto { get; set; }
        public string Audio { get; set; }
        public byte[] decodedStringAudio { get; set; }

    }
}

