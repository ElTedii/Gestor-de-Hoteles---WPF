using Cassandra;
using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class TipoHabitacionViewModel : ViewModelBase
    {
        private readonly TipoHabitacionRepository _repo;
        private readonly HotelRepository _hotelRepo;
        public int TotalHabitacionesTipo => Cantidad;
        public string PrecioPreview => $"MXN {PrecioNoche:n2}";

        // LISTAS
        public ObservableCollection<TipoHabitacionModel> TiposHabitacion { get; set; }
        public ObservableCollection<HotelModel> Hoteles { get; set; }

        // SELECCIONADOS
        private Guid _hotelSeleccionado;
        public Guid HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set { _hotelSeleccionado = value; OnPropertyChanged(nameof(HotelSeleccionado)); CargarTipos(); }
        }

        private TipoHabitacionModel _tipoSeleccionado;
        public TipoHabitacionModel TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                _tipoSeleccionado = value;
                OnPropertyChanged(nameof(TipoSeleccionado));
                OnPropertyChanged(nameof(PrecioPreview));

                if (value != null)
                    CargarFormulario(value);
            }
        }

        // CAMPOS DEL FORMULARIO
        public string NombreTipo { get; set; }
        public int Capacidad { get; set; }
        public decimal PrecioNoche { get; set; }
        public int Cantidad { get; set; }
        public string Nivel { get; set; }
        public string Vista { get; set; }

        public string CaracteristicasTexto { get; set; }
        public string AmenidadesTexto { get; set; }

        // COMANDOS
        public ICommand NuevoCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand RefrescarCommand { get; }

        // ----------------------------
        // CONSTRUCTOR
        // ----------------------------
        public TipoHabitacionViewModel()
        {
            _repo = new TipoHabitacionRepository();
            _hotelRepo = new HotelRepository();

            Hoteles = new ObservableCollection<HotelModel>(_hotelRepo.GetAll());
            TiposHabitacion = new ObservableCollection<TipoHabitacionModel>();

            if (Hoteles.Any())
                HotelSeleccionado = Hoteles.First().HotelId;

            NuevoCommand = new ViewModelCommand(_ => Nuevo());
            GuardarCommand = new ViewModelCommand(_ => Guardar());
            RefrescarCommand = new ViewModelCommand(_ => CargarTipos());
        }

        // ----------------------------
        // CARGAR LISTA
        // ----------------------------
        private void CargarTipos()
        {
            TiposHabitacion.Clear();

            if (HotelSeleccionado == Guid.Empty)
                return;

            var lista = _repo.GetByHotel(HotelSeleccionado);

            foreach (var t in lista)
                TiposHabitacion.Add(t);
        }

        // ----------------------------
        // NUEVO
        // ----------------------------
        private void Nuevo()
        {
            TipoSeleccionado = null;

            NombreTipo = "";
            Capacidad = 0;
            PrecioNoche = 0;
            Cantidad = 0;
            Nivel = "";
            Vista = "";
            CaracteristicasTexto = "";
            AmenidadesTexto = "";

            OnPropertyChanged(null); // refresca todo el formulario
        }

        // ----------------------------
        // GUARDAR
        // ----------------------------
        private void Guardar()
        {
            try
            {
                var modelo = TipoSeleccionado ?? new TipoHabitacionModel();

                modelo.HotelId = HotelSeleccionado;
                modelo.NombreTipo = NombreTipo;
                modelo.Capacidad = Capacidad;
                modelo.PrecioNoche = PrecioNoche;
                modelo.Cantidad = Cantidad;
                modelo.Nivel = Nivel;
                modelo.Vista = Vista;

                modelo.Caracteristicas = CaracteristicasTexto?
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).ToList()
                    ?? new();

                modelo.Amenidades = AmenidadesTexto?
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim()).ToList()
                    ?? new();

                modelo.UsuarioRegistro = LoginViewModel.UsuarioActual?.Correo ?? "sistema";

                if (modelo.TipoId == Guid.Empty)
                    _repo.Insert(modelo);
                else
                    _repo.Update(modelo);

                MessageBox.Show("Tipo guardado correctamente");
                CargarTipos();
                Nuevo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("[ERROR] " + ex.Message);
            }
            if (TipoDuplicado(NombreTipo))
            {
                MessageBox.Show("Ya existe un tipo de habitación con ese nombre.",
                                "Duplicado", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        // ----------------------------
        // Cargar en formulario
        // ----------------------------
        private void CargarFormulario(TipoHabitacionModel t)
        {
            NombreTipo = t.NombreTipo;
            Capacidad = t.Capacidad;
            PrecioNoche = t.PrecioNoche;
            Cantidad = t.Cantidad;
            Nivel = t.Nivel;
            Vista = t.Vista;

            CaracteristicasTexto = string.Join(", ", t.Caracteristicas ?? new());
            AmenidadesTexto = string.Join(", ", t.Amenidades ?? new());

            OnPropertyChanged(null);
        }

        private bool TipoDuplicado(string nombre)
        {
            return TiposHabitacion.Any(t =>
                t.NombreTipo.Trim().ToLower() == nombre.Trim().ToLower()
                && (TipoSeleccionado == null || t.TipoId != TipoSeleccionado.TipoId)
            );
        }
    }
}