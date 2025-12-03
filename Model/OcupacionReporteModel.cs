using System;

namespace Gestión_Hotelera.Model
{
    public class OcupacionReporteModel
    {
        public Guid HotelId { get; set; }
        public string HotelNombre { get; set; }

        public int Año { get; set; }
        public int Mes { get; set; }
        public string MesNombre { get; set; }

        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }

        public string TipoHabitacion { get; set; }
        public int NumeroHabitacion { get; set; }

        public DateTime FechaEntrada { get; set; }
        public DateTime FechaSalida { get; set; }

        public string Estado { get; set; } = "FINALIZADA";

        public decimal PagoHospedaje { get; set; }
        public decimal PagoServicios { get; set; }
    }
}