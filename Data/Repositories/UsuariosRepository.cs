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

        // INSERT
        public void Insert(UsersModel u)
        {
            var query = @"INSERT INTO usuarios (
                nomina, correo, contrasena, nombre_completo, fecha_nacimiento,
                tel_casa, tel_celular,
                usuario_creacion, fecha_creacion,
                usuario_modificacion, fecha_modificacion
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?);";

            var statement = _session.Prepare(query);

            _session.Execute(statement.Bind(
                u.Nomina, u.Correo, u.Pass, u.NombreCompleto, u.FechaNacimiento,
                u.TelCasa, u.TelCelular,
                u.UsuarioRegistro, u.FechaRegistro,
                u.UusuarioModificacion, u.FechaModificacion
            ));
        }

        // UPDATE
        public void Update(UsersModel u)
        {
            var query = @"UPDATE usuarios SET
                correo=?, contrasena=?, nombre_completo=?, fecha_nacimiento=?,
                tel_casa=?, tel_celular=?,
                usuario_modificacion=?, fecha_modificacion=?
                WHERE nomina=?;";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                u.Correo, u.Pass, u.NombreCompleto, u.FechaNacimiento,
                u.TelCasa, u.TelCelular,
                u.UusuarioModificacion, u.FechaModificacion,
                u.Nomina
            ));
        }

        // GET BY ID
        public UsersModel GetById(int nomina)
        {
            var query = "SELECT * FROM usuarios WHERE nomina = ?;";
            var row = _session.Execute(_session.Prepare(query).Bind(nomina)).FirstOrDefault();

            if (row == null) return null;

            return new UsersModel
            {
                Nomina = row.GetValue<int>("nomina"),
                Correo = row.GetValue<string>("correo"),
                Pass = row.GetValue<string>("contrasena"),
                NombreCompleto = row.GetValue<string>("nombre_completo"),
                FechaNacimiento = row.GetValue<DateTime>("fecha_nacimiento"),
                TelCasa = row.GetValue<string>("tel_casa"),
                TelCelular = row.GetValue<string>("tel_celular"),
                UsuarioRegistro = row.GetValue<string>("usuario_creacion"),
                FechaRegistro = row.GetValue<DateTime>("fecha_creacion"),
                UusuarioModificacion = row.GetValue<string>("usuario_modificacion"),
                FechaModificacion = row.GetValue<DateTime>("fecha_modificacion")
            };
        }

        // GET ALL
        public List<UsersModel> GetAll()
        {
            var result = _session.Execute("SELECT * FROM usuarios;");
            var lista = new List<UsersModel>();

            foreach (var row in result)
            {
                lista.Add(new UsersModel
                {
                    Nomina = row.GetValue<int>("nomina"),
                    Correo = row.GetValue<string>("correo"),
                    Pass = row.GetValue<string>("contrasena"),
                    NombreCompleto = row.GetValue<string>("nombre_completo"),
                    FechaNacimiento = row.GetValue<DateTime>("fecha_nacimiento"),
                    TelCasa = row.GetValue<string>("tel_casa"),
                    TelCelular = row.GetValue<string>("tel_celular"),
                    UsuarioRegistro = row.GetValue<string>("usuario_creacion"),
                    FechaRegistro = row.GetValue<DateTime>("fecha_creacion"),
                    UusuarioModificacion = row.GetValue<string>("usuario_modificacion"),
                    FechaModificacion = row.GetValue<DateTime>("fecha_modificacion")
                });
            }

            return lista;
        }

        // DELETE
        public void Delete(int nomina)
        {
            var query = "DELETE FROM usuarios WHERE nomina = ?;";
            _session.Execute(_session.Prepare(query).Bind(nomina));
        }

        // LOGIN BÁSICO
        public bool Login(string correo, string pass)
        {
            var query = "SELECT * FROM usuarios;";
            var usuarios = _session.Execute(query);

            foreach (var row in usuarios)
            {
                if (row.GetValue<string>("correo") == correo &&
                    row.GetValue<string>("contrasena") == pass)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
