using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class HabitacionesListaViewModel : ViewModelBase
    {
        private readonly HotelRepository hotelRepo = new HotelRepository();
        private readonly HabitacionRepository habRepo = new HabitacionRepository();
        private readonly TipoHabitacionRepository tipoRepo = new TipoHabitacionRepository();

        // ==============================
        //    LISTAS PRINCIPALES
        // ==============================
        public ObservableCollection<HotelModel> Hoteles { get; set; }
        public ObservableCollection<TipoHabitacionModel> Tipos { get; set; }
        public ObservableCollection<HabitacionModel> HabitacionesOriginal { get; set; }
        public ObservableCollection<HabitacionModel> HabitacionesFiltradas { get; set; }

        public ObservableCollection<string> Estados { get; set; } =
            new ObservableCollection<string> { "TODOS", "DISPONIBLE", "OCUPADA", "MANTENIMIENTO" };

        // ==============================
        //       PROPIEDADES FILTRO
        // ==============================
        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged();

                CargarTipos();
                CargarHabitaciones();
            }
        }

        private string _estadoSeleccionado = "TODOS";
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                _estadoSeleccionado = value;
                OnPropertyChanged();
                AplicarFiltros();
            }
        }

        private TipoHabitacionModel _tipoSeleccionado;
        public TipoHabitacionModel TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                _tipoSeleccionado = value;
                OnPropertyChanged();
                AplicarFiltros();
            }
        }

        private string _busquedaNumero;
        public string BusquedaNumero
        {
            get => _busquedaNumero;
            set
            {
                _busquedaNumero = value;
                OnPropertyChanged();
                AplicarFiltros();
            }
        }

        // ==============================
        //           COMMANDS
        // ==============================
        public ICommand RefrescarCommand { get; }

        // Acción para abrir el editor desde el View Principal
        public Action<HabitacionModel> EditarAction { get; set; }

        public ICommand EditarCommand { get; }

        // ==============================
        //         CONSTRUCTOR
        // ==============================
        public HabitacionesListaViewModel()
        {
            Hoteles = new ObservableCollection<HotelModel>(hotelRepo.GetAll());
            Tipos = new ObservableCollection<TipoHabitacionModel>();

            HabitacionesOriginal = new ObservableCollection<HabitacionModel>();
            HabitacionesFiltradas = new ObservableCollection<HabitacionModel>();

            RefrescarCommand = new ViewModelCommand(_ => CargarHabitaciones());

            EditarCommand = new ViewModelCommand(OnEditar);

            if (Hoteles.Any())
                HotelSeleccionado = Hoteles.First();
        }

        private void OnEditar(object obj)
        {
            if (obj is HabitacionModel h)
                EditarAction?.Invoke(h);
        }

        // ==============================
        //     CARGA DE DATOS
        // ==============================
        private void CargarTipos()
        {
            if (HotelSeleccionado == null)
            {
                Tipos.Clear();
                return;
            }

            var tipos = tipoRepo.GetByHotel(HotelSeleccionado.HotelId);
            Tipos = new ObservableCollection<TipoHabitacionModel>(tipos);
            OnPropertyChanged(nameof(Tipos));
        }

        public void CargarHabitaciones()
        {
            if (HotelSeleccionado == null)
                return;

            var lista = habRepo.GetByHotel(HotelSeleccionado.HotelId);

            HabitacionesOriginal = new ObservableCollection<HabitacionModel>(lista);
            AplicarFiltros();
        }

        // ==============================
        //       APLICACIÓN DE FILTROS
        // ==============================
        private void AplicarFiltros()
        {
            var lista = HabitacionesOriginal.AsEnumerable();

            // Filtro por estado
            if (EstadoSeleccionado != "TODOS")
                lista = lista.Where(h => h.Estado == EstadoSeleccionado);

            // Filtro por tipo
            if (TipoSeleccionado != null)
                lista = lista.Where(h => h.TipoId == TipoSeleccionado.TipoId);

            // Filtro por número
            if (!string.IsNullOrWhiteSpace(BusquedaNumero))
            {
                if (int.TryParse(BusquedaNumero, out int num))
                    lista = lista.Where(h => h.NumeroHabitacion == num);
            }

            HabitacionesFiltradas = new ObservableCollection<HabitacionModel>(lista);
            OnPropertyChanged(nameof(HabitacionesFiltradas));
        }
    }
}