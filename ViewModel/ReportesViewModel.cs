using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class ReportesViewModel : ViewModelBase
    {
        // ====== Repositorios ======
        private readonly HotelRepository _hotelRepo;
        private readonly OcupacionReportRepository _ocupacionRepo;
        // (futuros): VentasReportRepository, HistorialClienteReportRepository

        public ReportesViewModel()
        {
            _hotelRepo = new HotelRepository();
            _ocupacionRepo = new OcupacionReportRepository();

            Hoteles = new ObservableCollection<HotelModel>(_hotelRepo.GetAll());

            if (Hoteles.Any())
                HotelSeleccionado = Hoteles.First();

            AñoOcupacion = DateTime.Today.Year;
            MesOcupacion = DateTime.Today.Month;

            AñoVentas = DateTime.Today.Year;
            AñoHistorial = DateTime.Today.Year;

            OcupacionItems = new ObservableCollection<OcupacionReporteModel>();
            VentasItems = new ObservableCollection<VentasReporteModel>();
            HistorialItems = new ObservableCollection<HistorialClienteReporteModel>();

            BuscarOcupacionCommand = new ViewModelCommand(ExecuteBuscarOcupacion);
            BuscarVentasCommand = new ViewModelCommand(ExecuteBuscarVentas);
            BuscarHistorialCommand = new ViewModelCommand(ExecuteBuscarHistorial);

            // Cargar combos básicos para ventas (paises/ciudades) usando hoteles
            CargarFiltrosVentasDesdeHoteles();
        }

        // ================= OCUPACIÓN =================
        public ObservableCollection<HotelModel> Hoteles { get; }
        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set { _hotelSeleccionado = value; OnPropertyChanged(nameof(HotelSeleccionado)); }
        }

        private int _añoOcupacion;
        public int AñoOcupacion
        {
            get => _añoOcupacion;
            set { _añoOcupacion = value; OnPropertyChanged(nameof(AñoOcupacion)); }
        }

        private int _mesOcupacion;
        public int MesOcupacion
        {
            get => _mesOcupacion;
            set { _mesOcupacion = value; OnPropertyChanged(nameof(MesOcupacion)); }
        }

        public ObservableCollection<OcupacionReporteModel> OcupacionItems { get; }

        public ICommand BuscarOcupacionCommand { get; }

        private void ExecuteBuscarOcupacion(object obj)
        {
            if (HotelSeleccionado == null)
            {
                MessageBox.Show("Selecciona un hotel.");
                return;
            }

            try
            {
                var lista = _ocupacionRepo.GetOcupacion(
                    HotelSeleccionado.HotelId,
                    AñoOcupacion,
                    MesOcupacion
                );

                OcupacionItems.Clear();
                foreach (var item in lista)
                    OcupacionItems.Add(item);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al consultar ocupación: " + ex.Message);
            }
        }

        // ================= VENTAS (estructura lista, lógica luego) =================
        public ObservableCollection<string> Paises { get; private set; }
        public ObservableCollection<string> Ciudades { get; private set; }
        public ObservableCollection<HotelModel> HotelesVentas { get; private set; }

        private string _paisSeleccionado;
        public string PaisSeleccionado
        {
            get => _paisSeleccionado;
            set
            {
                _paisSeleccionado = value;
                OnPropertyChanged(nameof(PaisSeleccionado));
                CargarCiudades();
            }
        }

        private string _ciudadSeleccionada;
        public string CiudadSeleccionada
        {
            get => _ciudadSeleccionada;
            set
            {
                _ciudadSeleccionada = value;
                OnPropertyChanged(nameof(CiudadSeleccionada));
                CargarHotelesVentas();
            }
        }

        private HotelModel _hotelVentasSeleccionado;
        public HotelModel HotelVentasSeleccionado
        {
            get => _hotelVentasSeleccionado;
            set { _hotelVentasSeleccionado = value; OnPropertyChanged(nameof(HotelVentasSeleccionado)); }
        }

        private int _añoVentas;
        public int AñoVentas
        {
            get => _añoVentas;
            set { _añoVentas = value; OnPropertyChanged(nameof(AñoVentas)); }
        }

        public ObservableCollection<VentasReporteModel> VentasItems { get; }

        public ICommand BuscarVentasCommand { get; }

        private void CargarFiltrosVentasDesdeHoteles()
        {
            var hoteles = _hotelRepo.GetAll();

            var paises = hoteles
                .Select(h => h.Pais)
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            Paises = new ObservableCollection<string>(paises);
            Ciudades = new ObservableCollection<string>();
            HotelesVentas = new ObservableCollection<HotelModel>();

            OnPropertyChanged(nameof(Paises));
            OnPropertyChanged(nameof(Ciudades));
            OnPropertyChanged(nameof(HotelesVentas));
        }

        private void CargarCiudades()
        {
            Ciudades.Clear();
            HotelesVentas.Clear();

            if (string.IsNullOrWhiteSpace(PaisSeleccionado))
                return;

            var hoteles = _hotelRepo.GetAll()
                .Where(h => h.Pais == PaisSeleccionado)
                .ToList();

            foreach (var c in hoteles.Select(h => h.Ciudad).Distinct().OrderBy(c => c))
                Ciudades.Add(c);
        }

        private void CargarHotelesVentas()
        {
            HotelesVentas.Clear();

            if (string.IsNullOrWhiteSpace(PaisSeleccionado) ||
                string.IsNullOrWhiteSpace(CiudadSeleccionada))
                return;

            var hoteles = _hotelRepo.GetAll()
                .Where(h => h.Pais == PaisSeleccionado && h.Ciudad == CiudadSeleccionada)
                .OrderBy(h => h.Nombre)
                .ToList();

            foreach (var h in hoteles)
                HotelesVentas.Add(h);
        }

        private void ExecuteBuscarVentas(object obj)
        {
            // De momento lo dejamos vacío para que compile sin romper nada.
            // Aquí luego conectamos VentasReportRepository.
            VentasItems.Clear();
            MessageBox.Show("Lógica de ventas pendiente (estructura lista y compilando).");
        }

        // ================= HISTORIAL CLIENTE (estructura lista) =================
        private string _filtroCliente;
        public string FiltroCliente
        {
            get => _filtroCliente;
            set { _filtroCliente = value; OnPropertyChanged(nameof(FiltroCliente)); }
        }

        private int _añoHistorial;
        public int AñoHistorial
        {
            get => _añoHistorial;
            set { _añoHistorial = value; OnPropertyChanged(nameof(AñoHistorial)); }
        }

        public ObservableCollection<HistorialClienteReporteModel> HistorialItems { get; }

        public ICommand BuscarHistorialCommand { get; }

        private void ExecuteBuscarHistorial(object obj)
        {
            HistorialItems.Clear();
            MessageBox.Show("Lógica de historial de cliente pendiente (estructura lista y compilando).");
        }
    }
}