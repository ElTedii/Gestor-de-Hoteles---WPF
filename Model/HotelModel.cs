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
        public Guid HotelId { get; set; }              // hotel_id (PK)
        public string Nombre { get; set; }             // nombre
        public string Pais { get; set; }               // pais
        public string Estado { get; set; }             // estado
        public string Ciudad { get; set; }             // ciudad
        public string Domicilio { get; set; }          // domicilio
        public int NumPisos { get; set; }              // num_pisos
        public string ZonaTuristica { get; set; }      // zona_turistica
        public List<string> Servicios { get; set; }    // servicios (list<text>)
        public bool FrentePlaya { get; set; }          // frente_playa
        public int NumPiscinas { get; set; }           // num_piscinas
        public int SalonesEventos { get; set; }        // salones_eventos
        public int? UsuarioRegistro { get; set; }      // usuario_registro
        public DateTime? FechaRegistro { get; set; }   // fecha_registro
        public DateTime? FechaInicioOperaciones { get; set; } // fecha_inicio_op
        public DateTime? FechaModificacion { get; set; }      // fecha_modificacion

        public HotelModel()
        {
            Servicios = new List<string>();
        }
    }
}
