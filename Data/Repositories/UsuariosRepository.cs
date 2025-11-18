using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class UsuarioRepository
    {
        private readonly ISession _session;

        public UsuarioRepository()
        {
            _session = CassandraConnection.Session;
        }

        // Registrar usuario
        public bool InsertarUsuario(UsersModel u)
        {
            try
            {
                var query = _session.Prepare(@"
                    INSERT INTO usuarios
                    (nomina, correo, contrasena, nombre_completo,
                     fecha_nacimiento, tel_casa, tel_celular,
                     fecha_creacion, usuario_registro)
                    VALUES (?, ?, ?, ?, ?, ?, ?, toTimestamp(now()), ?)
                ");

                _session.Execute(query.Bind(
                    u.Nomina,
                    u.Correo,
                    u.Contrasena,
                    u.NombreCompleto,
                    u.FechaNacimiento,
                    u.TelefonoCasa,
                    u.TelefonoCelular,
                    u.UsuarioRegistro
                ));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error InsertarUsuario: " + ex.Message);
                return false;
            }
        }

        // Obtener usuario por correo
        public UsersModel ObtenerPorCorreo(string correo)
        {
            try
            {
                var query = _session.Prepare(@"
                    SELECT * FROM usuarios WHERE correo = ? ALLOW FILTERING;
                ");

                var result = _session.Execute(query.Bind(correo)).FirstOrDefault();
                if (result == null) return null;

                return Map(result);
            }
            catch
            {
                return null;
            }
        }

        // Obtener usuario por nómina
        public UsersModel ObtenerPorNomina(int nomina)
        {
            try
            {
                var query = _session.Prepare("SELECT * FROM usuarios WHERE nomina = ?;");
                var result = _session.Execute(query.Bind(nomina)).FirstOrDefault();

                return result != null ? Map(result) : null;
            }
            catch
            {
                return null;
            }
        }

        // Validar login
        public UsersModel ValidarLogin(string correo, string password)
        {
            try
            {
                var usuario = ObtenerPorCorreo(correo);
                if (usuario == null) return null;

                return usuario.Contrasena == password ? usuario : null;
            }
            catch
            {
                return null;
            }
        }

        // Map de Row → UsuarioModel
        private UsersModel Map(Row row)
        {
            return new UsersModel
            {
                Nomina = row.GetValue<int>("nomina"),
                Correo = row.GetValue<string>("correo"),
                Contrasena = row.GetValue<string>("contrasena"),
                NombreCompleto = row.GetValue<string>("nombre_completo"),
                FechaNacimiento = row.GetValue<DateTime>("fecha_nacimiento"),
                TelefonoCasa = row.GetValue<string>("tel_casa"),
                TelefonoCelular = row.GetValue<string>("tel_celular"),
                FechaCreacion = row.GetValue<DateTime>("fecha_creacion"),
                UsuarioRegistro = row.GetValue<int>("usuario_registro")
            };
        }
    }
}
