using Gestión_Hotelera.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Gestión_Hotelera.View
{
    /// <summary>
    /// Lógica de interacción para LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            var vm = new LoginViewModel();
            DataContext = vm;

            vm.CloseLoginAction = () => this.Close();
            vm.OpenMainWindowAction = () =>
            {
                MainView win = new MainView();
                win.Show();
            };
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var vm = (LoginViewModel)DataContext;

            // Pasamos usuario y contraseña al ViewModel
            vm.Correo = txtUser.Text;
            vm.Password = txtPass.Password;

            if (vm.LoginCommand.CanExecute(null))
            {
                vm.LoginCommand.Execute(null);

                // Si el ViewModel aprobó el login, MainWindow ya se abrió
                // Solo cerramos el login si fue correcto
                if (Application.Current.MainWindow is MainView)
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Ingrese correo y contraseña.", "Advertencia",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
