using System;
using System.Collections.Generic;
using System.Text;

namespace PM2E2GRUPO4.Models
{

public class Sitios
   {
		public int Id { get; set; }
        public string Descripcion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Foto { get; set; }
        public string Audio { get; set; }
    }
}
