using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.ViewModel
{
    public class HabitacionesViewModel : ViewModelBase
    {
        // ---- PROPIEDADES QUE CONTROLAN PESTAÑAS ----
        private int _selectedTabIndex = 0;
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set { _selectedTabIndex = value; OnPropertyChanged(); }
        }

        private string _registroTabVisible = "Collapsed";
        public string RegistroTabVisible
        {
            get => _registroTabVisible;
            set { _registroTabVisible = value; OnPropertyChanged(); }
        }

        // ---- VIEWMODELS INTERNOS ----
        public HabitacionesListaViewModel ListaVM { get; }
        public RegistroHabitacionViewModel RegistroVM { get; }
        public TipoHabitacionViewModel TiposVM { get; }

        // ---- COMMANDS ----
        public ICommand AbrirRegistroCommand { get; }
        public ICommand EditarHabitacionCommand { get; }
        public ICommand CerrarRegistroCommand { get; }

        public HabitacionesViewModel()
        {
            // Instancias internas
            ListaVM = new HabitacionesListaViewModel();
            RegistroVM = new RegistroHabitacionViewModel();
            TiposVM = new TipoHabitacionViewModel();

            // Eventos para comunicación
            ListaVM.EditarAction = AbrirEdicion;
            RegistroVM.CerrarAction = CerrarRegistro;

            // Commands
            AbrirRegistroCommand = new ViewModelCommand(_ => AbrirRegistroNuevo());
            CerrarRegistroCommand = new ViewModelCommand(_ => CerrarRegistro());
        }

        // ===============================
        //       MÉTODOS PRINCIPALES
        // ===============================

        private void AbrirRegistroNuevo()
        {
            RegistroTabVisible = "Visible";
            SelectedTabIndex = 1; // abre tab 2

            RegistroVM.Nuevo(); // limpia formulario
        }

        private void AbrirEdicion(HabitacionModel h)
        {
            RegistroTabVisible = "Visible";
            SelectedTabIndex = 1;

            RegistroVM.CargarDesdeLista(h);
        }

        private void CerrarRegistro()
        {
            RegistroTabVisible = "Collapsed";
            SelectedTabIndex = 0;

            ListaVM.CargarHabitaciones(); // recarga lista
        }
    }
}