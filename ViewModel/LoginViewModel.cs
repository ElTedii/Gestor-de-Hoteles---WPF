using Gestión_Hotelera.Model;
using Gestión_Hotelera.Data.Repositories;
using System;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        // Repositorio directo (simplificación del proyecto)
        private readonly UsuarioRepository _usuarioRepo;

        // Usuario logueado (sesión)
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
            _usuarioRepo = new UsuarioRepository();

            LoginCommand = new ViewModelCommand(ExecuteLogin, CanExecuteLogin);
        }

        private bool CanExecuteLogin(object obj)
        {
            return !string.IsNullOrWhiteSpace(Correo)
                && !string.IsNullOrWhiteSpace(Password);
        }

        private void ExecuteLogin(object obj)
        {
            // Buscar usuario por correo
            var usuario = _usuarioRepo.GetByCorreo(Correo);

            // Validar credenciales
            if (usuario == null || usuario.Contrasena != Password)
            {
                MessageBox.Show("Correo o contraseña incorrectos.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Guardar usuario en sesión
            UsuarioActual = usuario;

            // Abrir ventana principal
            OpenMainWindowAction?.Invoke();
            CloseLoginAction?.Invoke();
        }
    }
}
