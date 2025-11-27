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

        // ==========================================================
        // INSERT
        // ==========================================================
        public void Insert(ClienteModel c)
        {
            var query = @"INSERT INTO clientes (
                cliente_id, nombre_completo, pais, estado, ciudad,
                rfc, correo, tel_casa, tel_celular, fecha_nacimiento,
                estado_civil, usuario_creacion, fecha_creacion,
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
                c.FechaCreacion,
                c.UsuarioModifico,
                c.FechaModificacion
            ));

            // También insertamos en índices secundarios
            InsertClientePorCorreo(c);
            InsertClientePorRFC(c);
        }

        private void InsertClientePorCorreo(ClienteModel c)
        {
            var q = @"INSERT INTO clientes_por_correo (correo, cliente_id, nombre_completo)
                      VALUES (?,?,?);";

            _session.Execute(
                _session.Prepare(q).Bind(c.Correo, c.ClienteId, c.NombreCompleto)
            );
        }

        private void InsertClientePorRFC(ClienteModel c)
        {
            var q = @"INSERT INTO clientes_por_rfc (rfc, cliente_id, nombre_completo)
                      VALUES (?,?,?);";

            _session.Execute(
                _session.Prepare(q).Bind(c.RFC, c.ClienteId, c.NombreCompleto)
            );
        }

        // ==========================================================
        // UPDATE
        // ==========================================================
        public void Update(ClienteModel c)
        {
            var query = @"UPDATE clientes SET
                nombre_completo=?, pais=?, estado=?, ciudad=?,
                rfc=?, correo=?, tel_casa=?, tel_celular=?, fecha_nacimiento=?,
                estado_civil=?, usuario_modificacion=?, fecha_modificacion=?
                WHERE cliente_id=?;";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
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
                c.UsuarioModifico,
                c.FechaModificacion,
                c.ClienteId
            ));

            // Índices secundarios deben actualizarse también
            InsertClientePorCorreo(c);
            InsertClientePorRFC(c);
        }

        // ==========================================================
        // DELETE
        // ==========================================================
        public void Delete(Guid id)
        {
            var query = "DELETE FROM clientes WHERE cliente_id = ?";
            var stmt = _session.Prepare(query);
            _session.Execute(stmt.Bind(id));
        }

        // ==========================================================
        // QUERY: Buscar por correo
        // ==========================================================
        public ClienteModel GetByCorreo(string correo)
        {
            var idxRow = _session.Execute(
                _session.Prepare("SELECT cliente_id FROM clientes_por_correo WHERE correo=?;").Bind(correo)
            ).FirstOrDefault();

            if (idxRow == null) return null;

            return GetById(idxRow.GetValue<Guid>("cliente_id"));
        }

        // ==========================================================
        // QUERY: Buscar por RFC
        // ==========================================================
        public ClienteModel GetByRFC(string rfc)
        {
            var idxRow = _session.Execute(
                _session.Prepare("SELECT cliente_id FROM clientes_por_rfc WHERE rfc=?;").Bind(rfc)
            ).FirstOrDefault();

            if (idxRow == null) return null;

            return GetById(idxRow.GetValue<Guid>("cliente_id"));
        }

        // ==========================================================
        // QUERY: Buscar por ID
        // ==========================================================
        public ClienteModel GetById(Guid id)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM clientes WHERE cliente_id=?;").Bind(id)
            ).FirstOrDefault();

            if (row == null) return null;

            return MapRowToCliente(row);
        }

        // ==========================================================
        // GET ALL (solo para interfaz)
        // ==========================================================
        public List<ClienteModel> GetAll()
        {
            var result = _session.Execute("SELECT * FROM clientes;");
            var list = new List<ClienteModel>();

            foreach (var row in result)
                list.Add(MapRowToCliente(row));

            return list;
        }

        // ==========================================================
        // MAPEO
        // ==========================================================
        private ClienteModel MapRowToCliente(Row row)
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
                FechaNacimiento = row.GetValue<DateTime>("fecha_nacimiento"),
                EstadoCivil = row.GetValue<string>("estado_civil"),
                UsuarioRegistro = row.GetValue<string>("usuario_creacion"),
                FechaCreacion = row.GetValue<DateTime>("fecha_creacion"),
                UsuarioModifico = row.GetValue<string>("usuario_modificacion"),
                FechaModificacion = row.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}
