using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class HistorialClienteModel
    {
        public Guid ClienteId { get; set; }
        public int Año { get; set; }
        public DateTime FechaReserva { get; set; }
        public Guid ReservaId { get; set; }
        public Guid HotelId { get; set; }
        public string TipoHabitacion { get; set; }
        public int NumeroHabitacion { get; set; }
        public int Personas { get; set; }
        public DateTime FechaCheckIn { get; set; }
        public DateTime FechaCheckOut { get; set; }
        public string Estado { get; set; }
        public decimal Anticipo { get; set; }
        public decimal Hospedaje { get; set; }
        public decimal Servicios { get; set; }
        public decimal Total { get; set; }
    }
}
