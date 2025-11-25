using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class CheckInModel
    {
        public Guid ReservacionID { get; set; }
        public DateTime FechaCheckIn { get; set; }
        public List<int> HabitacionesAsignadas { get; set; }

        //Auditoria
        public string UsuarioRegistro { get; set; }
        public string FechaRegistro { get; set; }
    }
}
