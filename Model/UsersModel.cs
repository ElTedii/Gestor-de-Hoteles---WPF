using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class UsersModel
    {
        public int Nomina { get; set; }
        public string Correo { get; set; }
        public string Pass { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string TelCasa { get; set; }
        public string TelCelular { get; set; }

        public string UsuarioRegistro { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UusuarioModificacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}