using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestión_Hotelera.Data.Repositories
{
    public class ClienteRepository
    {
        private readonly ISession _session;

        public ClienteRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // INSERTAR
        public void Insert(ClienteModel c)
        {
            if (c.ClienteId == Guid.Empty)
                c.ClienteId = Guid.NewGuid();

            c.FechaRegistro = DateTime.UtcNow;

            const string query = @"
                INSERT INTO clientes (
                    cliente_id, nombre_completo, pais, estado, ciudad,
                    rfc, correo, tel_casa, tel_celular, fecha_nacimiento,
                    estado_civil, usuario_registro, fecha_registro,
                    usuario_modificacion, fecha_modificacion
                ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?);";

            _session.Execute(_session.Prepare(query).Bind(
                c.ClienteId, c.NombreCompleto, c.Pais, c.Estado, c.Ciudad,
                c.RFC, c.Correo, c.TelCasa, c.TelCelular, c.FechaNacimiento,
                c.EstadoCivil, c.UsuarioRegistro, c.FechaRegistro,
                c.UsuarioModificacion, c.FechaModificacion
            ));
        }

        // ACTUALIZAR
        public void Update(ClienteModel c)
        {
            c.UsuarioModificacion ??= c.UsuarioRegistro;
            c.FechaModificacion = DateTime.UtcNow;

            const string q = @"
                UPDATE clientes SET
                    nombre_completo=?, pais=?, estado=?, ciudad=?,
                    rfc=?, correo=?, tel_casa=?, tel_celular=?, 
                    fecha_nacimiento=?, estado_civil=?,
                    usuario_modificacion=?, fecha_modificacion=?
                WHERE cliente_id=?;";

            _session.Execute(_session.Prepare(q).Bind(
                c.NombreCompleto, c.Pais, c.Estado, c.Ciudad,
                c.RFC, c.Correo, c.TelCasa, c.TelCelular,
                c.FechaNacimiento, c.EstadoCivil,
                c.UsuarioModificacion, c.FechaModificacion,
                c.ClienteId
            ));
        }

        // OBTENER TODO
        public List<ClienteModel> GetAll()
        {
            var rows = _session.Execute("SELECT * FROM clientes;");

            return rows.Select(MapRow).ToList();
        }

        // OBTENER POR ID
        public ClienteModel GetById(Guid id)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM clientes WHERE cliente_id=?;")
                .Bind(id)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // MAPEO
        private ClienteModel MapRow(Row r)
        {
            return new ClienteModel
            {
                ClienteId = r.GetValue<Guid>("cliente_id"),
                NombreCompleto = r.GetValue<string>("nombre_completo"),
                Pais = r.GetValue<string>("pais"),
                Estado = r.GetValue<string>("estado"),
                Ciudad = r.GetValue<string>("ciudad"),

                RFC = r.GetValue<string>("rfc"),
                Correo = r.GetValue<string>("correo"),
                TelCasa = r.GetValue<string>("tel_casa"),
                TelCelular = r.GetValue<string>("tel_celular"),

                FechaNacimiento = r.IsNull("fecha_nacimiento")
                    ? (DateTime?)null
                    : r.GetValue<DateTime>("fecha_nacimiento"),

                EstadoCivil = r.GetValue<string>("estado_civil"),

                UsuarioRegistro = r.GetValue<string>("usuario_registro"),
                FechaRegistro = r.GetValue<DateTime>("fecha_registro"),

                UsuarioModificacion = r.GetValue<string>("usuario_modificacion"),
                FechaModificacion = r.IsNull("fecha_modificacion")
                    ? (DateTime?)null
                    : r.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}