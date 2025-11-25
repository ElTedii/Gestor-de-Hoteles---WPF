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
        public string NombreHotel { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Ciudad { get; set; }
        public string Domicilio { get; set; }
        public int NumeroPisos { get; set; }

        // Listas serializables (json)
        public List<string> TiposHabitacion { get; set; }
        public List<string> Caracteristicas { get; set; }
        public List<string> Amenidades { get; set; }
        public List<string> ServiciosAdicionales { get; set; }

        public string ZonaTuristica { get; set; }
        public bool FrentePlaya { get; set; }
        public int CantidadPiscinas { get; set; }
        public bool SalonesEventos { get; set; }

        public DateTime FechaInicioOperaciones { get; set; }

        // Auditoría
        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}
