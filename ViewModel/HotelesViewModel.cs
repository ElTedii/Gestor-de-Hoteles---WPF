using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class HotelesViewModel : ViewModelBase
    {
        private readonly HotelRepository _repo = new HotelRepository();

        // ==============================
        // LISTA PRINCIPAL
        // ==============================
        public ObservableCollection<HotelModel> Hoteles { get; set; }

        private HotelModel _hotelSeleccionado;
        public HotelModel HotelSeleccionado
        {
            get => _hotelSeleccionado;
            set
            {
                _hotelSeleccionado = value;
                OnPropertyChanged();

                if (value != null)
                    CargarFormulario(value);
            }
        }

        // ==============================
        // PROPIEDADES DEL FORMULARIO
        // ==============================
        public string Nombre { get; set; }
        public string Pais { get; set; }
        public string Estado { get; set; }
        public string Ciudad { get; set; }
        public string Domicilio { get; set; }

        public int NumPisos { get; set; }
        public string ZonaTuristica { get; set; }

        public bool FrentePlaya { get; set; }
        public int NumPiscinas { get; set; }
        public int SalonesEventos { get; set; }

        private Guid _hotelIdActual;

        // ==============================
        // COMMANDS
        // ==============================
        public ICommand NuevoCommand { get; }
        public ICommand GuardarCommand { get; }
        public ICommand EliminarCommand { get; }
        public ICommand RefrescarCommand { get; }

        // ==============================
        // CONSTRUCTOR
        // ==============================
        public HotelesViewModel()
        {
            Hoteles = new ObservableCollection<HotelModel>(_repo.GetAll());

            NuevoCommand = new ViewModelCommand(_ => Nuevo());
            GuardarCommand = new ViewModelCommand(_ => Guardar());
            EliminarCommand = new ViewModelCommand(_ => Eliminar());
            RefrescarCommand = new ViewModelCommand(_ => Refrescar());

            Nuevo();
        }

        // ==============================
        // MÉTODOS PRINCIPALES
        // ==============================
        private void Nuevo()
        {
            _hotelIdActual = Guid.Empty;

            Nombre = "";
            Pais = "";
            Estado = "";
            Ciudad = "";
            Domicilio = "";
            NumPisos = 1;
            ZonaTuristica = "";
            FrentePlaya = false;
            NumPiscinas = 0;
            SalonesEventos = 0;

            OnPropertyChanged(nameof(Nombre));
            OnPropertyChanged(nameof(Pais));
            OnPropertyChanged(nameof(Estado));
            OnPropertyChanged(nameof(Ciudad));
            OnPropertyChanged(nameof(Domicilio));
            OnPropertyChanged(nameof(NumPisos));
            OnPropertyChanged(nameof(ZonaTuristica));
            OnPropertyChanged(nameof(FrentePlaya));
            OnPropertyChanged(nameof(NumPiscinas));
            OnPropertyChanged(nameof(SalonesEventos));
        }

        private void CargarFormulario(HotelModel h)
        {
            _hotelIdActual = h.HotelId;

            Nombre = h.Nombre;
            Pais = h.Pais;
            Estado = h.Estado;
            Ciudad = h.Ciudad;
            Domicilio = h.Domicilio;

            NumPisos = h.NumPisos;
            ZonaTuristica = h.ZonaTuristica;
            FrentePlaya = h.FrentePlaya;

            NumPiscinas = h.NumPiscinas;
            SalonesEventos = h.SalonesEventos;

            OnPropertyChanged(nameof(Nombre));
            OnPropertyChanged(nameof(Pais));
            OnPropertyChanged(nameof(Estado));
            OnPropertyChanged(nameof(Ciudad));
            OnPropertyChanged(nameof(Domicilio));
            OnPropertyChanged(nameof(NumPisos));
            OnPropertyChanged(nameof(ZonaTuristica));
            OnPropertyChanged(nameof(FrentePlaya));
            OnPropertyChanged(nameof(NumPiscinas));
            OnPropertyChanged(nameof(SalonesEventos));
        }

        private void Guardar()
        {
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                MessageBox.Show("El nombre es obligatorio");
                return;
            }

            var h = new HotelModel
            {
                HotelId = _hotelIdActual,
                Nombre = Nombre,
                Pais = Pais,
                Estado = Estado,
                Ciudad = Ciudad,
                Domicilio = Domicilio,
                NumPisos = NumPisos,
                ZonaTuristica = ZonaTuristica,
                FrentePlaya = FrentePlaya,
                NumPiscinas = NumPiscinas,
                SalonesEventos = SalonesEventos,

                UsuarioRegistro = "admin",
                UsuarioModificacion = "admin",
                FechaRegistro = DateTime.UtcNow,
                FechaModificacion = DateTime.UtcNow
            };

            _repo.InsertOrUpdate(h);
            MessageBox.Show("Hotel guardado correctamente.");

            Refrescar();
            Nuevo();
        }

        private void Eliminar()
        {
            if (_hotelIdActual == Guid.Empty)
            {
                MessageBox.Show("Selecciona un hotel.");
                return;
            }

            if (MessageBox.Show("¿Eliminar hotel?", "Confirmar",
                MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            _repo.Delete(_hotelIdActual);
            Refrescar();
            Nuevo();
        }

        private void Refrescar()
        {
            Hoteles = new ObservableCollection<HotelModel>(_repo.GetAll());
            OnPropertyChanged(nameof(Hoteles));
        }
    }
}