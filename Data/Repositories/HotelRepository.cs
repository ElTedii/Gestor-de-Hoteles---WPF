using Cassandra;
using Cassandra.Mapping;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gestión_Hotelera.Data.Repositories
{
    public class HotelRepository
    {
        private readonly ISession _session;

        public HotelRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // ==========================================================
        // INSERT
        // ==========================================================
        public void Insert(HotelModel h)
        {
            var query = @"INSERT INTO hoteles (
                hotel_id, nombre, pais, estado, ciudad, domicilio,
                num_pisos, zona_turistica, servicios, frente_playa,
                num_piscinas, salones_eventos,
                usuario_registro, fecha_registro, fecha_modificacion
            ) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?);";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                h.HotelId,
                h.Nombre,
                h.Pais,
                h.Estado,
                h.Ciudad,
                h.Domicilio,
                h.NumPisos,
                h.ZonaTuristica,
                h.Servicios,
                h.FrentePlaya,
                h.NumPiscinas,
                h.SalonesEventos,
                h.UsuarioRegistro,
                h.FechaRegistro,        // fecha_registro
                h.FechaModificacion
            ));
        }

        // ==========================================================
        // UPDATE
        // ==========================================================
        public void Update(HotelModel h)
        {
            var query = @"UPDATE hoteles SET
                nombre=?, pais=?, estado=?, ciudad=?, domicilio=?,
                num_pisos=?, zona_turistica=?, servicios=?, frente_playa=?,
                num_piscinas=?, salones_eventos=?,
                fecha_modificacion=?
                WHERE hotel_id=?;";

            var stmt = _session.Prepare(query);

            _session.Execute(stmt.Bind(
                h.Nombre,
                h.Pais,
                h.Estado,
                h.Ciudad,
                h.Domicilio,
                h.NumPisos,
                h.ZonaTuristica,
                h.Servicios,
                h.FrentePlaya,
                h.NumPiscinas,
                h.SalonesEventos,
                h.FechaModificacion,
                h.HotelId
            ));
        }

        // ==========================================================
        // DELETE
        // ==========================================================
        public void Delete(Guid hotelId)
        {
            var stmt = _session.Prepare("DELETE FROM hoteles WHERE hotel_id=?;");
            _session.Execute(stmt.Bind(hotelId));
        }

        // ==========================================================
        // GET BY ID
        // ==========================================================
        public HotelModel GetById(Guid id)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM hoteles WHERE hotel_id=?;").Bind(id)
            ).FirstOrDefault();

            return row != null ? MapRow(row) : null;
        }

        // ==========================================================
        // GET ALL
        // ==========================================================
        public List<HotelModel> GetAll()
        {
            var results = _session.Execute("SELECT * FROM hoteles;");
            var list = new List<HotelModel>();

            foreach (var row in results)
                list.Add(MapRow(row));

            return list;
        }

        // ==========================================================
        // MAPEO
        // ==========================================================
        private HotelModel MapRow(Row row)
        {
            return new HotelModel
            {
                HotelId = row.GetValue<Guid>("hotel_id"),
                Nombre = row.GetValue<string>("nombre"),
                Pais = row.GetValue<string>("pais"),
                Estado = row.GetValue<string>("estado"),
                Ciudad = row.GetValue<string>("ciudad"),
                Domicilio = row.GetValue<string>("domicilio"),
                NumPisos = row.GetValue<int>("num_pisos"),
                ZonaTuristica = row.GetValue<string>("zona_turistica"),
                Servicios = row.GetValue<List<string>>("servicios"),
                FrentePlaya = row.GetValue<bool>("frente_playa"),
                NumPiscinas = row.GetValue<int>("num_piscinas"),
                SalonesEventos = row.GetValue<int>("salones_eventos"),

                UsuarioRegistro = row.GetValue<string>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime>("fecha_registro"),
                FechaModificacion = row.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}
