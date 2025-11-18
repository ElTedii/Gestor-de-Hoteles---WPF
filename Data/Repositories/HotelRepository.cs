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
            _session = CassandraConnection.Session;
        }

        public async Task InsertarHotelAsync(HotelModel h)
        {
            h.HotelId = Guid.NewGuid();

            var query = @"INSERT INTO hoteles
            (hotel_id, nombre, pais, estado, ciudad, domicilio, num_pisos, zona_turistica,
             servicios, frente_playa, num_piscinas, salones_eventos, usuario_registro,
             fecha_registro, fecha_inicio_op, fecha_modificacion)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, toTimestamp(now()), ?, toTimestamp(now()));";

            var ps = _session.Prepare(query);

            await _session.ExecuteAsync(ps.Bind(
                h.HotelId, h.Nombre, h.Pais, h.Estado, h.Ciudad, h.Domicilio, h.NumPisos,
                h.ZonaTuristica, h.Servicios, h.FrentePlaya, h.NumPiscinas, h.SalonesEventos,
                h.UsuarioRegistro, h.FechaInicioOperaciones
            ));
        }

        public async Task<IEnumerable<HotelModel>> ObtenerTodosAsync()
        {
            var result = await _session.ExecuteAsync(new SimpleStatement("SELECT * FROM hoteles"));

            return result.Select(row => new HotelModel
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
                UsuarioRegistro = row.GetValue<int>("usuario_registro"),
                FechaRegistro = row.GetValue<DateTime?>("fecha_registro"),
                FechaInicioOperaciones = row.GetValue<DateTime?>("fecha_inicio_op")
            });
        }

        public async Task ActualizarHotelAsync(HotelModel h)
        {
            var query = @"UPDATE hoteles SET
                nombre = ?, pais = ?, estado = ?, ciudad = ?, domicilio = ?,
                num_pisos = ?, zona_turistica = ?, servicios = ?, frente_playa = ?,
                num_piscinas = ?, salones_eventos = ?, fecha_modificacion = toTimestamp(now())
                WHERE hotel_id = ?";

            var ps = _session.Prepare(query);

            await _session.ExecuteAsync(ps.Bind(
                h.Nombre, h.Pais, h.Estado, h.Ciudad, h.Domicilio, h.NumPisos, h.ZonaTuristica,
                h.Servicios, h.FrentePlaya, h.NumPiscinas, h.SalonesEventos, h.HotelId
            ));
        }

        public async Task EliminarHotelAsync(Guid hotelId)
        {
            await _session.ExecuteAsync(
                _session.Prepare("DELETE FROM hoteles WHERE hotel_id = ?").Bind(hotelId)
            );
        }
    }
}
