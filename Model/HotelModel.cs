using System;
using System.Collections.Generic;

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

        public List<string> Servicios { get; set; } = new List<string>();

        public bool FrentePlaya { get; set; }
        public int NumPiscinas { get; set; }
        public int SalonesEventos { get; set; }

        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioModificacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}