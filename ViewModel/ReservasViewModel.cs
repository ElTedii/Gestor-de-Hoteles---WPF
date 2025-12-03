using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class ReservasViewModel : ViewModelBase
    {
        private readonly HotelRepository _hotelRepo = new HotelRepository();
        private readonly ReservasPorHotelRepository _reservaHotelRepo = new ReservasPorHotelRepository();
        private readonly ReservaRepository _reservaRepo = new ReservaRepository();
        private readonly ClienteRepository _clienteRepo = new ClienteRepository();

        // Estas acciones las configura MainViewModel
        public Action NuevaReservacionAction { get; set; }
        public Action<ReservaListadoModel> EditarAction { get; set; }

        // ===============================
        // COMBOS
        // ===============================
        public ObservableCollection<HotelModel> Hoteles { get; set; }
        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged();
                CargarReservaciones();
            }
        }

        public ObservableCollection<string> Estados { get; set; } =
            new ObservableCollection<string>
            {
                "TODOS",
                "PENDIENTE",
                "CONFIRMADA",
                "EN_ESTANCIA",
                "FINALIZADA"
            };

        private string _estadoSeleccionado = "TODOS";
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                _estadoSeleccionado = value;
                OnPropertyChanged();
                CargarReservaciones();
            }
        }

        // ===============================
        // LISTA
        // ===============================
        public ObservableCollection<ReservaListadoModel> Reservaciones { get; set; }

        // ===============================
        // COMMANDS
        // ===============================
        public ICommand NuevaCommand { get; }
        public ICommand EditarCommand { get; }

        public ReservasViewModel()
        {
            Hoteles = new ObservableCollection<HotelModel>(_hotelRepo.GetAll());
            Reservaciones = new ObservableCollection<ReservaListadoModel>();

            NuevaCommand = new ViewModelCommand(_ => NuevaReservacionAction?.Invoke());
            EditarCommand = new ViewModelCommand(r =>
            {
                var modelo = r as ReservaListadoModel;
                if (modelo != null)
                    EditarAction?.Invoke(modelo);
            });

            // Seleccionar primer hotel por defecto
            HotelSeleccionado = Hoteles.FirstOrDefault();
        }

        // ===============================
        // CARGAR RESERVAS
        // ===============================
        private void CargarReservaciones()
        {
            Reservaciones.Clear();

            if (HotelSeleccionado == null)
                return;

            // Base desde tabla reservas_por_hotel
            var lista = _reservaHotelRepo.GetListadoByHotel(HotelSeleccionado.HotelId);

            // Filtro por estado
            if (EstadoSeleccionado != "TODOS")
                lista = lista.Where(r => r.Estado == EstadoSeleccionado).ToList();

            foreach (var r in lista)
            {
                // Enriquecer info con reservas_por_cliente
                var detalle = _reservaRepo.GetByClienteAndReserva(r.ClienteId, r.ReservaId);
                var cliente = _clienteRepo.GetById(r.ClienteId);

                // Cliente y hotel
                r.ClienteNombre = cliente?.NombreCompleto ?? "—";
                r.HotelNombre = HotelSeleccionado.Nombre;

                if (detalle != null)
                {
                    r.Adultos = detalle.Adultos;
                    r.Menores = detalle.Menores;
                    r.NumPersonas = detalle.Adultos + detalle.Menores;
                    r.Anticipo = detalle.Anticipo;

                    // Por ahora usamos anticipo como “total” mostrado
                    // (si después guardas precio_total en Cassandra, aquí lo sustituyes)
                    if (r.PrecioTotal == 0)
                        r.PrecioTotal = detalle.Anticipo;
                }

                Reservaciones.Add(r);
            }
        }
    }
}