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
            _session = CassandraConnection.GetSession();
        }

        // ============================================================
        // OBTENER POR CORREO (LOGIN)
        // ============================================================
        public UsersModel GetByCorreo(string correo)
        {
            var query = "SELECT * FROM usuarios WHERE correo = ?;";
            var row = _session.Execute(_session.Prepare(query).Bind(correo)).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // INSERTAR
        // ============================================================
        public void Insert(UsersModel u)
        {
            const string query = @"
            INSERT INTO usuarios (
                correo, contrasena, nombre_completo,
                nomina, fecha_nacimiento,
                tel_casa, tel_celular,
                usuario_creacion, fecha_creacion,
                usuario_modificacion, fecha_modificacion
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?);";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                u.Correo,
                u.Contrasena,
                u.NombreCompleto,
                u.Nomina,
                u.FechaNacimiento,
                u.TelCasa,
                u.TelCelular,
                u.UsuarioCreacion,
                u.FechaCreacion,
                u.UsuarioModificacion,
                u.FechaModificacion
            ));
        }

        // ============================================================
        // MAPEO GENERAL
        // ============================================================
        private UsersModel MapRow(Row row)
        {
            return new UsersModel
            {
                Correo = row.GetValue<string>("correo"),
                Contrasena = row.GetValue<string>("contrasena"),
                NombreCompleto = row.GetValue<string>("nombre_completo"),

                Nomina = row.GetValue<int?>("nomina") ?? 0,
                FechaNacimiento = row.GetValue<DateTime?>("fecha_nacimiento"),

                TelCasa = row.GetValue<string>("tel_casa"),
                TelCelular = row.GetValue<string>("tel_celular"),

                UsuarioCreacion = row.GetValue<string>("usuario_creacion"),
                FechaCreacion = row.GetValue<DateTime>("fecha_creacion"),
                UsuarioModificacion = row.GetValue<string>("usuario_modificacion"),
                FechaModificacion = row.GetValue<DateTime?>("fecha_modificacion"),
            };
        }
    }
}
