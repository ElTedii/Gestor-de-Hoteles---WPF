using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gestión_Hotelera.View
{
    /// <summary>
    /// Lógica de interacción para RegistroHotelView.xaml
    /// </summary>
    public partial class RegistroHotelView : UserControl
    {
        public ObservableCollection<string> TiposHabitacion { get; set; }

        public RegistroHotelView()
        {
            InitializeComponent();

            TiposHabitacion = new ObservableCollection<string>();
            ItemsTipos.ItemsSource = TiposHabitacion;

            BtnAddTipo.Click += (s, e) =>
            {
                TiposHabitacion.Add(string.Empty);
            };
        }
    }
}
