using FontAwesome.Sharp;
using Gestión_Hotelera.Model;
using System;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        public ViewModelBase CurrentChildView
        {
            get => _currentChildView;
            set { _currentChildView = value; OnPropertyChanged(nameof(CurrentChildView)); }
        }

        public string Caption
        {
            get => _caption;
            set { _caption = value; OnPropertyChanged(nameof(Caption)); }
        }

        public IconChar Icon
        {
            get => _icon;
            set { _icon = value; OnPropertyChanged(nameof(Icon)); }
        }

        // Commands
        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowClientViewCommand { get; }
        public ICommand ShowHotelsViewCommand { get; }
        public ICommand ShowRegistroHotelViewCommand { get; }
        public ICommand ShowHabitacionesViewCommand { get; }
        public ICommand ShowRegistroHabitacionViewCommand { get; }
        public ICommand ShowRegistroClienteCommand { get; }
        public ICommand ShowReservasViewCommand { get; }
        public ICommand ShowNuevaReservaCommand { get; }

        public ICommand ShowCheckInCommand { get; }
        public ICommand ShowCheckOutcommand { get; }
        public ICommand ShowRealizarCheckOutCommand { get; }

        public MainViewModel()
        {
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowClientViewCommand = new ViewModelCommand(ExecuteShowClientViewCommand);
            ShowHotelsViewCommand = new ViewModelCommand(ExecuteShowHotelsViewCommand);
            ShowRegistroHotelViewCommand = new ViewModelCommand(ExecuteShowRegistroHotelViewCommand);
            ShowHabitacionesViewCommand = new ViewModelCommand(ExecuteShowHabitacionesViewCommand);
            ShowRegistroHabitacionViewCommand = new ViewModelCommand(ExecuteShowRegistroHabitacionViewCommand);
            ShowRegistroClienteCommand = new ViewModelCommand(ExecuteShowRegistroClienteCommand);

            ShowReservasViewCommand = new ViewModelCommand(ExecuteShowReservas);
            ShowNuevaReservaCommand = new ViewModelCommand(ExecuteShowNuevaReserva);

            ShowCheckInCommand = new ViewModelCommand(ExecuteShowCheckInCommand);
            ShowCheckOutcommand = new ViewModelCommand(ExecuteShowCheckOutcommand);
            ShowRealizarCheckOutCommand = new ViewModelCommand(ExecuteShowRealziarCheckOutcommand);

            ExecuteShowHomeViewCommand(null);
        }

        // ===================== VISTAS PRINCIPALES ======================
        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = "Inicio";
            Icon = IconChar.Home;
        }

        private void ExecuteShowClientViewCommand(object obj)
        {
            CurrentChildView = new ClientesViewModel(this);
            Caption = "Clientes";
            Icon = IconChar.User;
        }

        private void ExecuteShowHotelsViewCommand(object obj)
        {
            CurrentChildView = new HotelesViewModel(this);
            Caption = "Hoteles";
            Icon = IconChar.Building;
        }

        private void ExecuteShowRegistroHotelViewCommand(object obj)
        {
            CurrentChildView = new RegistroHotelViewModel(this);
            Caption = "Registrar Hotel";
            Icon = IconChar.Building;
        }

        private void ExecuteShowHabitacionesViewCommand(object obj)
        {
            CurrentChildView = new HabitacionesViewModel(this);
            Caption = "Habitaciones";
            Icon = IconChar.Bed;
        }

        private void ExecuteShowRegistroHabitacionViewCommand(object obj)
        {
            CurrentChildView = new RegistroHabitacionViewModel();
            Caption = "Registrar Habitación";
            Icon = IconChar.Bed;
        }

        private void ExecuteShowRegistroClienteCommand(object obj)
        {
            CurrentChildView = new RegistroClienteViewModel(this);
            Caption = "Registrar Cliente";
            Icon = IconChar.UserPlus;
        }

        private void ExecuteShowReservas(object obj)
        {
            var vm = new ReservasViewModel();
            vm.AbrirNuevaReservaAction = () =>
            {
                CurrentChildView = new NuevaReservaViewModel(this);
                Caption = "Nueva Reservación";
                Icon = IconChar.PlusCircle;
            };

            CurrentChildView = vm;
            Caption = "Reservaciones";
            Icon = IconChar.CalendarCheck;
        }

        private void ExecuteShowNuevaReserva(object obj)
        {
            CurrentChildView = new NuevaReservaViewModel(this);
            Caption = "Nueva Reservación";
            Icon = IconChar.PlusCircle;
        }

        // ===================== CHECK-IN ======================
        private void ExecuteShowCheckInCommand(object obj)
        {
            var vm = new CheckInViewModel();
            vm.AbrirRealizarCheckInAction = AbrirVentanaRealizarCheckIn;

            CurrentChildView = vm;
            Caption = "Check In";
            Icon = IconChar.Check;
        }

        private void AbrirVentanaRealizarCheckIn(RealizarCheckInModel model)
        {
            var vm = new RealizarCheckInViewModel(model, this);

            // Cuando el usuario cierre el Check-In, regresamos a la lista de Check-In
            vm.CloseAction = () => ExecuteShowCheckInCommand(null);

            CurrentChildView = vm;
            Caption = "Realizar Check In";
            Icon = IconChar.Check;
        }

        // ===================== CHECK-OUT ======================
        private void ExecuteShowCheckOutcommand(object obj)
        {
            CurrentChildView = new CheckOutViewModel();
            Caption = "Check Out";
            Icon = IconChar.CheckDouble;
        }

        private void ExecuteShowRealziarCheckOutcommand(object obj)
        {
            var vm = new RealizarCheckOutViewModel
            {
                CloseAction = () => ExecuteShowCheckOutcommand(null)
            };

            CurrentChildView = vm;
            Caption = "Realizar Check Out";
            Icon = IconChar.CheckDouble;
        }

        // ===================== PERFIL ======================
        public string UserEmail => LoginViewModel.UsuarioActual?.Correo ?? "No email";

        public string UserInitials
        {
            get
            {
                var nombre = LoginViewModel.UsuarioActual?.NombreCompleto ?? "U";
                var partes = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                if (partes.Length == 0) return "U";
                if (partes.Length == 1) return partes[0][0].ToString().ToUpper();

                return (partes[0][0].ToString() + partes[1][0].ToString()).ToUpper();
            }
        }
    }
}