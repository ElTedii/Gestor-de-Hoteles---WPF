using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class ClienteModel
    {
        public Guid ClienteId { get; set; }
        public string NombreCompleto { get; set; }

        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Ciudad { get; set; }

        public string RFC { get; set; }
        public string Correo { get; set; }
        public string TelCasa { get; set; }
        public string TelCelular { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string EstadoCivil { get; set; }

        // Auditoría
        public string UsuarioRegistro { get; set; }      // usuario_creacion
        public string UsuarioModifico { get; set; }      // usuario_modificacion
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
    }
}