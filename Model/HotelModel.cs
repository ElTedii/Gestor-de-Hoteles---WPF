using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class HotelModel
    {
        public Guid HotelId { get; set; }
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Ciudad { get; set; }
        public string Domicilio { get; set; }
        public int NumPisos { get; set; }
        public string ZonaTuristica { get; set; }
        public List<string> Servicios { get; set; }
        public bool FrentePlaya { get; set; }
        public int NumPiscinas { get; set; }
        public int SalonesEventos { get; set; }
        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
