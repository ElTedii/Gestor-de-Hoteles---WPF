using System;

namespace Gestión_Hotelera.Model
{
    public class HistorialClienteReporteModel
    {
        public Guid ClienteId { get; set; }
        public string ClienteNombre { get; set; }

        public string Ciudad { get; set; }
        public string HotelNombre { get; set; }

        public string TipoHabitacion { get; set; }
        public int NumeroHabitacion { get; set; }
        public int Personas { get; set; }

        public DateTime FechaReserva { get; set; }
        public DateTime FechaCheckIn { get; set; }
        public DateTime FechaCheckOut { get; set; }

        public string Estado { get; set; }

        public decimal Anticipo { get; set; }
        public decimal Hospedaje { get; set; }
        public decimal Servicios { get; set; }
        public decimal Total { get; set; }
    }
}