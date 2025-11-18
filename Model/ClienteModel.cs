using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Model
{
    public class ClienteModel
    {
        public Guid ClienteId { get; set; }            // cliente_id (PK)
        public string NombreCompleto { get; set; }     // nombre_completo
        public string Pais { get; set; }               // pais
        public string Estado { get; set; }             // estado
        public string Ciudad { get; set; }             // ciudad
        public string RFC { get; set; }                // rfc
        public string Correo { get; set; }             // correo
        public string TelefonoCasa { get; set; }       // tel_casa
        public string TelefonoCelular { get; set; }    // tel_celular
        public DateTime? FechaNacimiento { get; set; } // fecha_nacimiento
        public string EstadoCivil { get; set; }        // estado_civil
        public int? UsuarioRegistro { get; set; }      // usuario_registro (numero_nomina)
        public int? UsuarioModifico { get; set; }      // usuario_modifico
        public DateTime? FechaCreacion { get; set; }   // fecha_creacion
        public DateTime? FechaModificacion { get; set; } // fecha_modificacion
    }

    public class ClientePorCorreoModel
    {
        public string Correo { get; set; }         // PK
        public Guid ClienteId { get; set; }
        public string NombreCompleto { get; set; }
    }

    public class ClientePorRFCModel
    {
        public string RFC { get; set; }            // PK
        public Guid ClienteId { get; set; }
        public string NombreCompleto { get; set; }
    }
}