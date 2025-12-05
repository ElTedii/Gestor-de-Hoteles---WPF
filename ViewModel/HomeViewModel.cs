using FontAwesome.Sharp;
using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly HotelRepository _hotelRepo;
        private readonly ReservaRepository _reservaRepo;
        private readonly EstanciaActivaRepository _estanciaRepo;
        private readonly FacturaRepository _facturaRepo;
        private readonly ClienteRepository _clienteRepo;

        public DateTime CurrentDate => DateTime.Now;

        public ObservableCollection<DashboardCardModel> Cards { get; set; }
        public ObservableCollection<ReservaListadoModel> LastReservations { get; set; }

        public ICommand SeeAllReservationsCommand { get; }

        public HomeViewModel()
        {
            _hotelRepo = new HotelRepository();
            _reservaRepo = new ReservaRepository();
            _estanciaRepo = new EstanciaActivaRepository();
            _facturaRepo = new FacturaRepository();
            _clienteRepo = new ClienteRepository();

            Cards = new ObservableCollection<DashboardCardModel>();
            LastReservations = new ObservableCollection<ReservaListadoModel>();

            SeeAllReservationsCommand = new ViewModelCommand(ExecuteSeeAll);

            CargarTarjetas();
            CargarReservasDelDia();
        }

        private void ExecuteSeeAll(object obj)
        {
            // Navegación futura
        }

        // ======================================================
        // TARJETAS DEL DASHBOARD
        // ======================================================
        private void CargarTarjetas()
        {
            var hoy = DateTime.UtcNow.Date;
            var manana = hoy.AddDays(1);

            // 1) Total hoteles
            int totalHoteles = _hotelRepo.GetAll().Count;

            // 2) Reservas cuya FECHA_ENTRADA es HOY
            var reservasHoy = _reservaRepo.GetAll()
                .Count(r => r.FechaEntrada >= hoy &&
                            r.FechaEntrada < manana);

            // 3) Check-Ins HOY
            var checkinsHoy = _estanciaRepo.GetCheckInsDelDia(hoy).Count;

            // 4) Ingresos del mes
            var primerDiaMes = new DateTime(DateTime.UtcNow.Year,
                                            DateTime.UtcNow.Month, 1);

            decimal ingresosMes = _facturaRepo.GetIngresosDesde(primerDiaMes);

            Cards.Clear();

            Cards.Add(new DashboardCardModel
            {
                Titulo = "Hoteles",
                Valor = totalHoteles.ToString(),
                Icono = IconChar.Building,
                Color = "#6D2FFF"
            });

            Cards.Add(new DashboardCardModel
            {
                Titulo = "Reservas Hoy",
                Valor = reservasHoy.ToString(),
                Icono = IconChar.CalendarDay,
                Color = "#FB539B"
            });

            Cards.Add(new DashboardCardModel
            {
                Titulo = "Check-Ins Hoy",
                Valor = checkinsHoy.ToString(),
                Icono = IconChar.Check,
                Color = "#4ECB71"
            });

            Cards.Add(new DashboardCardModel
            {
                Titulo = "Ingresos Mes",
                Valor = ingresosMes.ToString("C"),
                Icono = IconChar.MoneyBill,
                Color = "#FFD93D"
            });
        }

        // ======================================================
        // LISTA DE RESERVAS DEL DÍA (para el grid de abajo)
        // ======================================================
        private void CargarReservasDelDia()
        {
            LastReservations.Clear();

            var hoy = DateTime.UtcNow.Date;
            var manana = hoy.AddDays(1);

            // Reservas cuya fecha_entrada es HOY
            var reservasHoy = _reservaRepo.GetAll()
                .Where(r => r.FechaEntrada >= hoy &&
                            r.FechaEntrada < manana)
                .ToList();

            foreach (var r in reservasHoy)
            {
                var cliente = _clienteRepo.GetById(r.ClienteId);
                var hotel = _hotelRepo.GetById(r.HotelId);

                LastReservations.Add(new ReservaListadoModel
                {
                    ReservaId = r.ReservaId,
                    ClienteId = r.ClienteId,
                    HotelId = r.HotelId,

                    ClienteNombre = cliente?.NombreCompleto ?? "N/D",
                    HotelNombre = hotel?.Nombre ?? "N/D",

                    FechaEntrada = r.FechaEntrada,
                    FechaSalida = r.FechaSalida,

                    // Anticipo como valor representativo
                    PrecioTotal = r.Anticipo
                });
            }
        }
    }
}