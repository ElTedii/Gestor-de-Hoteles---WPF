using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class RegistroHabitacionViewModel : ViewModelBase
    {
        // REPOS
        private readonly HotelRepository _hotelRepo = new HotelRepository();
        private readonly TipoHabitacionRepository _tipoRepo = new TipoHabitacionRepository();
        private readonly HabitacionRepository _habRepo = new HabitacionRepository();

        // Acción opcional para avisar al padre (si la usas)
        public Action CerrarAction { get; set; }

        // ============================
        //  LISTAS PARA COMBOS Y GRID
        // ============================
        public ObservableCollection<HotelModel> Hoteles { get; set; }
        public ObservableCollection<TipoHabitacionModel> TiposHabitacion { get; set; }
        public ObservableCollection<int> Pisos { get; set; }
        public ObservableCollection<string> Estados { get; set; }
        public ObservableCollection<HabitacionModel> HabitacionesRegistradas { get; set; }

        // ============================
        //  PROPIEDADES DEL FORMULARIO
        // ============================
        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged();

                CargarTiposHabitacion();
                CargarHabitaciones();
                CargarPisos();
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

                // Precio automático según tipo
                PrecioNoche = _tipoSeleccionado?.PrecioNoche ?? 0;
            }
        }

        private int _numeroHabitacion;
        public int NumeroHabitacion
        {
            get => _numeroHabitacion;
            set { _numeroHabitacion = value; OnPropertyChanged(); }
        }

        private int _piso;
        public int Piso
        {
            get => _piso;
            set { _piso = value; OnPropertyChanged(); }
        }

        private string _estado = "DISPONIBLE";
        public string Estado
        {
            get => _estado;
            set { _estado = value; OnPropertyChanged(); }
        }

        private decimal _precioNoche;
        public decimal PrecioNoche
        {
            get => _precioNoche;
            set { _precioNoche = value; OnPropertyChanged(); }
        }

        private HabitacionModel _habitacionSeleccionada;
        public HabitacionModel HabitacionSeleccionada
        {
            get => _habitacionSeleccionada;
            set
            {
                _habitacionSeleccionada = value;
                OnPropertyChanged();
            }
        }

        // ============================
        //       COMMANDS
        // ============================
        public ICommand GuardarCommand { get; }
        public ICommand NuevoCommand { get; }
        public ICommand RefrescarCommand { get; }

        // ============================
        //     CONSTRUCTOR
        // ============================
        public RegistroHabitacionViewModel()
        {
            Hoteles = new ObservableCollection<HotelModel>(_hotelRepo.GetAll());
            TiposHabitacion = new ObservableCollection<TipoHabitacionModel>();
            Pisos = new ObservableCollection<int>();
            Estados = new ObservableCollection<string>
            {
                "DISPONIBLE",
                "OCUPADA",
                "MANTENIMIENTO"
            };
            HabitacionesRegistradas = new ObservableCollection<HabitacionModel>();

            GuardarCommand = new ViewModelCommand(_ => Guardar());
            NuevoCommand = new ViewModelCommand(_ => Nuevo());
            RefrescarCommand = new ViewModelCommand(_ => Refrescar());

            // Si quieres seleccionar el primer hotel automáticamente:
            if (Hoteles.Any())
                HotelSeleccionado = Hoteles.First();

            Nuevo();
        }

        // ============================
        //        MÉTODOS
        // ============================
        private void CargarTiposHabitacion()
        {
            TiposHabitacion.Clear();

            if (HotelSeleccionado == null)
            {
                OnPropertyChanged(nameof(TiposHabitacion));
                return;
            }

            var tipos = _tipoRepo.GetByHotel(HotelSeleccionado.HotelId);
            foreach (var t in tipos)
                TiposHabitacion.Add(t);

            OnPropertyChanged(nameof(TiposHabitacion));
        }

        private void CargarHabitaciones()
        {
            HabitacionesRegistradas.Clear();

            if (HotelSeleccionado == null)
            {
                OnPropertyChanged(nameof(HabitacionesRegistradas));
                return;
            }

            var list = _habRepo.GetByHotel(HotelSeleccionado.HotelId);

            foreach (var h in list)
                HabitacionesRegistradas.Add(h);

            OnPropertyChanged(nameof(HabitacionesRegistradas));
        }

        private void CargarPisos()
        {
            Pisos.Clear();

            if (HotelSeleccionado == null)
            {
                OnPropertyChanged(nameof(Pisos));
                return;
            }

            // Si tu HotelModel tiene NumPisos, úsalo.
            // Si no, deja fijo 10 o 20.
            int numPisos = 10;
            try
            {
                // si existe la propiedad:
                var prop = typeof(HotelModel).GetProperty("NumPisos");
                if (prop != null)
                {
                    var valor = prop.GetValue(HotelSeleccionado);
                    if (valor is int np && np > 0)
                        numPisos = np;
                }
            }
            catch { }

            for (int i = 1; i <= numPisos; i++)
                Pisos.Add(i);

            // Si el piso actual no está dentro de rango, lo reseteamos
            if (!Pisos.Contains(Piso))
                Piso = Pisos.FirstOrDefault();

            OnPropertyChanged(nameof(Pisos));
        }

        public void Nuevo()
        {
            HabitacionSeleccionada = null;

            NumeroHabitacion = 0;
            Piso = Pisos.Any() ? Pisos.First() : 1;
            Estado = "DISPONIBLE";
            TipoSeleccionado = null;
            PrecioNoche = 0;
        }

        private void Guardar()
        {
            if (HotelSeleccionado == null)
            {
                ShowMessage("Selecciona un hotel.");
                return;
            }

            if (TipoSeleccionado == null)
            {
                ShowMessage("Selecciona un tipo de habitación.");
                return;
            }

            if (NumeroHabitacion <= 0)
            {
                ShowMessage("El número de habitación debe ser mayor a 0.");
                return;
            }

            // Validar número repetido
            var existe = _habRepo.GetByHotelAndNumero(HotelSeleccionado.HotelId, NumeroHabitacion) != null;

            if (existe)
            {
                MessageBox.Show("Ya existe una habitación con ese número.");
                return;
            }

            var usuario = LoginViewModel.UsuarioActual?.Correo ?? "sistema";
            var ahora = DateTime.UtcNow;

            var h = new HabitacionModel
            {
                HotelId = HotelSeleccionado.HotelId,
                NumeroHabitacion = NumeroHabitacion,
                TipoId = TipoSeleccionado.TipoId,
                Piso = Piso,
                Estado = Estado,
                PrecioNoche = PrecioNoche,
                TipoNombre = TipoSeleccionado.NombreTipo,

                UsuarioRegistro = usuario,
                FechaRegistro = ahora,
                UsuarioModificacion = usuario,
                FechaModificacion = ahora
            };

            _habRepo.Insert(h);

            ShowMessage("Habitación registrada correctamente.");
            CargarHabitaciones();
            Nuevo();
        }

        private void Refrescar()
        {
            CargarTiposHabitacion();
            CargarHabitaciones();
            CargarPisos();
            ShowMessage("Información actualizada.");
        }

        public void CargarDesdeLista(HabitacionModel h)
        {
            if (h == null)
                return;

            // Selecciona hotel
            HotelSeleccionado = Hoteles.FirstOrDefault(x => x.HotelId == h.HotelId);

            // Cargar tipos de ese hotel
            CargarTiposHabitacion();

            // Seleccionar tipo
            TipoSeleccionado = TiposHabitacion.FirstOrDefault(x => x.TipoId == h.TipoId);

            NumeroHabitacion = h.NumeroHabitacion;
            Piso = h.Piso;
            Estado = string.IsNullOrEmpty(h.Estado) ? "DISPONIBLE" : h.Estado;
            PrecioNoche = h.PrecioNoche;
        }

        private void ShowMessage(string msg)
        {
            System.Windows.MessageBox.Show(
                msg,
                "Kuma Hotel",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }
    }
}