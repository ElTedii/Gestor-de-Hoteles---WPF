using FontAwesome.Sharp;
using Gestión_Hotelera.Model;
using Gestión_Hotelera.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        // Fields
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        // Properties
        public ViewModelBase CurrentChildView
        {
            get => _currentChildView;
            set
            {
                _currentChildView = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }

        public string Caption
        {
            get => _caption;
            set
            {
                _caption = value;
                OnPropertyChanged(nameof(Caption));
            }
        }

        public IconChar Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
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
        public ICommand ShowEditarReservaCommand { get; }
        public ICommand ShowCheckInCommand { get; }
        public ICommand ShowRealizarCheckInCommand { get; }
        public ICommand ShowCheckOutcommand { get; }
        public ICommand ShowRealizarCheckOutCommand {  get; }

        public MainViewModel()
        {
            // Initialize commands
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowClientViewCommand = new ViewModelCommand(ExecuteShowClientViewCommand);
            ShowHotelsViewCommand = new ViewModelCommand(ExecuteShowHotelsViewCommand);
            ShowRegistroHotelViewCommand = new ViewModelCommand(ExecuteShowRegistroHotelViewCommand);
            ShowHabitacionesViewCommand = new ViewModelCommand(ExecuteShowHabitacionesViewCommand);
            ShowRegistroHabitacionViewCommand = new ViewModelCommand(ExecuteShowRegistroHabitacionViewCommand);
            ShowRegistroClienteCommand = new ViewModelCommand(ExecuteShowRegistroClienteCommand);
            ShowReservasViewCommand = new ViewModelCommand(ExecuteShowReservas);
            ShowNuevaReservaCommand = new ViewModelCommand(ExecuteShowNuevaReserva);
            ShowEditarReservaCommand = new ViewModelCommand(ExecuteShowEditarReserva);
            ShowCheckInCommand = new ViewModelCommand(ExecuteShowCheckInCommand);
            ShowRealizarCheckInCommand = new ViewModelCommand(ExecuteShowShowRealizarCheckInCommand);
            ShowCheckOutcommand = new ViewModelCommand(ExecuteShowCheckOutcommand);
            ShowRealizarCheckOutCommand = new ViewModelCommand(ExecuteShowRealziarCheckOutcommand);

            // Default view
            ExecuteShowHomeViewCommand(null);
        }


        // Navigation methods
        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = "Inicio";
            Icon = IconChar.Home;
        }

        private void ExecuteShowClientViewCommand(object obj)
        {
            CurrentChildView = new ClientViewModel(this);
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
            CurrentChildView = new RegistroHotelViewModel();
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
            Caption = "Registrar habitación";
            Icon = IconChar.Bed;
        }

        private void ExecuteShowRegistroClienteCommand(object obj)
        {
            CurrentChildView = new RegistroClienteViewModel();
            Caption = "Registro Cliente";
            Icon = IconChar.UserPlus;
        }

        private void ExecuteShowReservas(object obj)
        {
            CurrentChildView = new ReservasViewModel(this);
            Caption = "Reservaciones";
            Icon = IconChar.CalendarCheck;
        }

        private void ExecuteShowNuevaReserva(object obj)
        {
            CurrentChildView = new NuevaReservaViewModel(this);
            Caption = "Nueva Reservación";
            Icon = IconChar.PlusCircle;
        }

        private void ExecuteShowEditarReserva(object obj)
        {
            if (obj is ReservationModel r)
            {
                CurrentChildView = new EditarReservaViewModel(this, r);
                Caption = "Editar Reservación";
                Icon = IconChar.Edit;
            }
        }

        private void ExecuteShowCheckInCommand(object obj)
        {
            CurrentChildView = new CheckInViewModel();
            Caption = "Check In";
            Icon = IconChar.Check;
        }

        private void ExecuteShowShowRealizarCheckInCommand(object obj)
        {
            CurrentChildView = new RealizarCheckInViewModel();
            Caption = "Realizar Check In";
            Icon = IconChar.Check;
        }

        private void ExecuteShowCheckOutcommand(object obj)
        {
            CurrentChildView = new CheckOutViewModel();
            Caption = "Check Out";
            Icon = IconChar.Check;
        }

        private void ExecuteShowRealziarCheckOutcommand(object obj)
        {
            CurrentChildView = new RealizarCheckOutViewModel();
            Caption = "Realizar Check Out";
            Icon = IconChar.Check;
        }

    }
}