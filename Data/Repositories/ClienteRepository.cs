using Gestión_Hotelera.Model;
using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class ClienteRepository
    {
        private readonly ISession _session;

        public ClienteRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ============================================================
        // INSERTAR CLIENTE
        // ============================================================
        public void Insert(ClienteModel c)
        {
            if (c.ClienteId == Guid.Empty)
                c.ClienteId = Guid.NewGuid();

            if (c.FechaRegistro == default)
                c.FechaRegistro = DateTime.UtcNow;

            const string query = @"
            INSERT INTO clientes (
                cliente_id, nombre_completo,
                pais, estado, ciudad,
                rfc, correo, tel_casa, tel_celular,
                fecha_nacimiento, estado_civil,
                usuario_registro, fecha_registro,
                usuario_modificacion, fecha_modificacion
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?);";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                c.ClienteId,
                c.NombreCompleto,
                c.Pais,
                c.Estado,
                c.Ciudad,
                c.RFC,
                c.Correo,
                c.TelCasa,
                c.TelCelular,
                c.FechaNacimiento,
                c.EstadoCivil,
                c.UsuarioRegistro,
                c.FechaRegistro,
                c.UsuarioModificacion,
                c.FechaModificacion
            ));
        }

        // ============================================================
        // OBTENER POR ID
        // ============================================================
        public ClienteModel GetById(Guid id)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM clientes WHERE cliente_id=?;")
                .Bind(id)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // ============================================================
        // OBTENER TODOS
        // ============================================================
        public List<ClienteModel> GetAll()
        {
            var rs = _session.Execute("SELECT * FROM clientes;");

            return rs.Select(MapRow).ToList();
        }

        // ============================================================
        // ACTUALIZAR CLIENTE
        // ============================================================
        public void Update(ClienteModel c)
        {
            c.FechaModificacion = DateTime.UtcNow;

            const string query = @"
            UPDATE clientes SET
                nombre_completo=?,
                pais=?, estado=?, ciudad=?,
                rfc=?, correo=?, tel_casa=?, tel_celular=?,
                fecha_nacimiento=?, estado_civil=?,
                usuario_modificacion=?, fecha_modificacion=?
            WHERE cliente_id=?;";

            _session.Execute(_session.Prepare(query).Bind(
                c.NombreCompleto,
                c.Pais,
                c.Estado,
                c.Ciudad,
                c.RFC,
                c.Correo,
                c.TelCasa,
                c.TelCelular,
                c.FechaNacimiento,
                c.EstadoCivil,
                c.UsuarioModificacion,
                c.FechaModificacion,
                c.ClienteId
            ));
        }

        // ============================================================
        // ELIMINAR CLIENTE
        // ============================================================
        public void Delete(Guid clienteId)
        {
            _session.Execute(
                _session.Prepare("DELETE FROM clientes WHERE cliente_id=?;")
                .Bind(clienteId)
            );
        }

        // ============================================================
        // MAPEO
        // ============================================================
        private ClienteModel MapRow(Row row)
        {
            return new ClienteModel
            {
                ClienteId = row.GetValue<Guid>("cliente_id"),
                NombreCompleto = row.GetValue<string>("nombre_completo"),

                Pais = row.GetValue<string>("pais"),
                Estado = row.GetValue<string>("estado"),
                Ciudad = row.GetValue<string>("ciudad"),

                RFC = row.GetValue<string>("rfc"),
                Correo = row.GetValue<string>("correo"),
                TelCasa = row.GetValue<string>("tel_casa"),
                TelCelular = row.GetValue<string>("tel_celular"),

                FechaNacimiento = row.GetValue<DateTime?>("fecha_nacimiento"),
                EstadoCivil = row.GetValue<string>("estado_civil"),

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime>("fecha_registro"),
                UsuarioModificacion = row.GetValue<string>("usuario_modificacion"),
                FechaModificacion = row.GetValue<DateTime?>("fecha_modificacion")
            };
        }
    }
}
