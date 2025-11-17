using FontAwesome.Sharp;
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

        public MainViewModel()
        {
            // Initialize commands
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);
            ShowClientViewCommand = new ViewModelCommand(ExecuteShowClientViewCommand);
            ShowHotelsViewCommand = new ViewModelCommand(ExecuteShowHotelsViewCommand);
            ShowRegistroHotelViewCommand = new ViewModelCommand(ExecuteShowRegistroHotelViewCommand);
            ShowHabitacionesViewCommand = new ViewModelCommand(ExecuteShowHabitacionesViewCommand);
            ShowRegistroHabitacionViewCommand = new ViewModelCommand(ExecuteShowRegistroHabitacionViewCommand);

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
            CurrentChildView = new ClientViewModel();
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
    }
}