using Gestión_Hotelera.Model;
using Gestión_Hotelera.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        // Servicio de usuarios (no repositorio directo)
        private readonly UsuarioService _usuarioService;

        // Usuario logueado
        public static UsersModel UsuarioActual { get; private set; }

        // PROPIEDADES PARA LOGIN
        private string _correo;
        public string Correo
        {
            get => _correo;
            set { _correo = value; OnPropertyChanged(nameof(Correo)); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(nameof(Password)); }
        }

        // Commands
        public ICommand LoginCommand { get; }

        public Action CloseLoginAction { get; set; }
        public Action OpenMainWindowAction { get; set; }

        public LoginViewModel()
        {
            _usuarioService = new UsuarioService();

            LoginCommand = new ViewModelCommand(ExecuteLogin, CanExecuteLogin);
        }

        private bool CanExecuteLogin(object obj)
        {
            return !string.IsNullOrWhiteSpace(Correo)
                && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object obj)
        {
            // Llama al servicio, no al repo
            var usuario = _usuarioService.Login(Correo, Password);

            if (usuario == null)
            {
                MessageBox.Show("Correo o contraseña incorrectos.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Guardar usuario en sesión estática (opcional)
            UsuarioActual = usuario;

            // Abrir ventana principal
            OpenMainWindowAction?.Invoke();
            CloseLoginAction?.Invoke();
        }
    }
}
