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
            _session = CassandraConnection.Session;
        }

        // INSERT
        public async Task InsertarClienteAsync(ClienteModel c)
        {
            c.ClienteId = Guid.NewGuid();

            var query1 = @"INSERT INTO clientes
            (cliente_id, nombre_completo, pais, estado, ciudad, rfc, correo,
             tel_casa, tel_celular, fecha_nacimiento, estado_civil,
             usuario_registro, usuario_modifico, fecha_creacion, fecha_modificacion)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, toTimestamp(now()), toTimestamp(now()));";

            var ps1 = _session.Prepare(query1);

            await _session.ExecuteAsync(ps1.Bind(
                c.ClienteId, c.NombreCompleto, c.Pais, c.Estado, c.Ciudad, c.RFC, c.Correo,
                c.TelefonoCasa, c.TelefonoCelular, c.FechaNacimiento, c.EstadoCivil,
                c.UsuarioRegistro, c.UsuarioModifico
            ));

            var query2 = @"INSERT INTO clientes_por_correo (correo, cliente_id, nombre_completo)
                           VALUES (?, ?, ?)";
            await _session.ExecuteAsync(_session.Prepare(query2).Bind(c.Correo, c.ClienteId, c.NombreCompleto));

            var query3 = @"INSERT INTO clientes_por_rfc (rfc, cliente_id, nombre_completo)
                           VALUES (?, ?, ?)";
            await _session.ExecuteAsync(_session.Prepare(query3).Bind(c.RFC, c.ClienteId, c.NombreCompleto));
        }

        // SELECT
        public async Task<ClienteModel> BuscarPorCorreoAsync(string correo)
        {
            var query = "SELECT * FROM clientes_por_correo WHERE correo = ?";
            var ps = _session.Prepare(query);
            var row = (await _session.ExecuteAsync(ps.Bind(correo))).FirstOrDefault();
            if (row == null) return null;

            return await ObtenerClienteAsync(row.GetValue<Guid>("cliente_id"));
        }

        public async Task<ClienteModel> BuscarPorRFCAsync(string rfc)
        {
            var query = "SELECT * FROM clientes_por_rfc WHERE rfc = ?";
            var ps = _session.Prepare(query);
            var row = (await _session.ExecuteAsync(ps.Bind(rfc))).FirstOrDefault();
            if (row == null) return null;

            return await ObtenerClienteAsync(row.GetValue<Guid>("cliente_id"));
        }

        public async Task<ClienteModel> ObtenerClienteAsync(Guid id)
        {
            var query = "SELECT * FROM clientes WHERE cliente_id = ?";
            var ps = _session.Prepare(query);
            var rs = await _session.ExecuteAsync(ps.Bind(id));
            var row = rs.FirstOrDefault();
            if (row == null) return null;

            return new ClienteModel
            {
                ClienteId = id,
                NombreCompleto = row.GetValue<string>("nombre_completo"),
                Pais = row.GetValue<string>("pais"),
                Estado = row.GetValue<string>("estado"),
                Ciudad = row.GetValue<string>("ciudad"),
                RFC = row.GetValue<string>("rfc"),
                Correo = row.GetValue<string>("correo"),
                TelefonoCasa = row.GetValue<string>("tel_casa"),
                TelefonoCelular = row.GetValue<string>("tel_celular"),
                FechaNacimiento = row.GetValue<DateTime?>("fecha_nacimiento"),
                EstadoCivil = row.GetValue<string>("estado_civil"),
                UsuarioRegistro = row.GetValue<int?>("usuario_registro"),
                UsuarioModifico = row.GetValue<int?>("usuario_modifico"),
                FechaCreacion = row.GetValue<DateTime?>("fecha_creacion"),
                FechaModificacion = row.GetValue<DateTime?>("fecha_modificacion")
            };
        }

        // UPDATE
        public async Task ActualizarClienteAsync(ClienteModel c)
        {
            var query = @"UPDATE clientes SET
                nombre_completo = ?, pais = ?, estado = ?, ciudad = ?, rfc = ?, correo = ?,
                tel_casa = ?, tel_celular = ?, fecha_nacimiento = ?, estado_civil = ?,
                usuario_modifico = ?, fecha_modificacion = toTimestamp(now())
                WHERE cliente_id = ?";

            var ps = _session.Prepare(query);

            await _session.ExecuteAsync(ps.Bind(
                c.NombreCompleto, c.Pais, c.Estado, c.Ciudad, c.RFC, c.Correo,
                c.TelefonoCasa, c.TelefonoCelular, c.FechaNacimiento, c.EstadoCivil,
                c.UsuarioModifico, c.ClienteId
            ));
        }
    }
}
