using Cassandra;
using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gestión_Hotelera.Data.Repositories
{
    public class HotelRepository
    {
        private readonly ISession _session;

        public HotelRepository()
        {
            _session = CassandraConnection.GetSession();
        }

        // =====================================================
        // LISTA COMPLETA
        // =====================================================
        public List<HotelModel> GetAll()
        {
            var rows = _session.Execute("SELECT * FROM hoteles;");
            return rows.Select(MapRow).ToList();
        }

        // =====================================================
        // INSERT OR UPDATE
        // =====================================================
        public void InsertOrUpdate(HotelModel h)
        {
            if (h.HotelId == Guid.Empty)
                h.HotelId = Guid.NewGuid();

            h.FechaModificacion = DateTime.UtcNow;
            if (h.FechaRegistro == default)
                h.FechaRegistro = DateTime.UtcNow;

            const string q = @"
                INSERT INTO hoteles (
                    hotel_id, nombre, pais, estado, ciudad,
                    domicilio, num_pisos, zona_turistica,
                    servicios, frente_playa, num_piscinas,
                    salones_eventos, usuario_registro, fecha_registro,
                    usuario_modificacion, fecha_modificacion
                ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

            _session.Execute(
                _session.Prepare(q).Bind(
                    h.HotelId, h.Nombre, h.Pais, h.Estado, h.Ciudad,
                    h.Domicilio, h.NumPisos, h.ZonaTuristica,
                    h.Servicios, h.FrentePlaya, h.NumPiscinas,
                    h.SalonesEventos, h.UsuarioRegistro, h.FechaRegistro,
                    h.UsuarioModificacion, h.FechaModificacion
                )
            );
        }

        // =====================================================
        // DELETE
        // =====================================================
        public void Delete(Guid hotelId)
        {
            _session.Execute(
                _session.Prepare("DELETE FROM hoteles WHERE hotel_id=?;")
                .Bind(hotelId)
            );
        }

        public HotelModel GetById(Guid hotelId)
        {
            var row = _session.Execute(
                _session.Prepare("SELECT * FROM hoteles WHERE hotel_id=?;")
                        .Bind(hotelId)
            ).FirstOrDefault();

            return row == null ? null : MapRow(row);
        }

        // =====================================================
        // MAPEO
        // =====================================================
        private HotelModel MapRow(Row r)
        {
            return new HotelModel
            {
                HotelId = r.GetValue<Guid>("hotel_id"),
                Nombre = r.GetValue<string>("nombre"),
                Pais = r.GetValue<string>("pais"),
                Estado = r.GetValue<string>("estado"),
                Ciudad = r.GetValue<string>("ciudad"),
                Domicilio = r.GetValue<string>("domicilio"),

                NumPisos = r.GetValue<int>("num_pisos"),

                ZonaTuristica = r.GetValue<string>("zona_turistica"),
                Servicios = r.GetValue<List<string>>("servicios"),
                FrentePlaya = r.GetValue<bool>("frente_playa"),
                NumPiscinas = r.GetValue<int>("num_piscinas"),
                SalonesEventos = r.GetValue<int>("salones_eventos"),

                UsuarioRegistro = r.GetValue<string>("usuario_registro"),

                FechaRegistro = r.IsNull("fecha_registro")
                    ? DateTime.UtcNow
                    : r.GetValue<DateTime>("fecha_registro"),

                UsuarioModificacion = r.IsNull("usuario_modificacion")
                    ? null
                    : r.GetValue<string>("usuario_modificacion"),

                FechaModificacion = r.IsNull("fecha_modificacion")
                    ? (DateTime?)null
                    : r.GetValue<DateTime>("fecha_modificacion")
            };
        }
    }
}