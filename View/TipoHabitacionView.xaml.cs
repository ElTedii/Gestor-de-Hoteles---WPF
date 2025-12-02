using Gestión_Hotelera.ViewModel;
using System.Windows.Controls;

namespace Gestión_Hotelera.View
{
    public partial class TipoHabitacionView : UserControl
    {
        public TipoHabitacionView()
        {
            InitializeComponent();
            DataContext = new TipoHabitacionViewModel();
        }
    }
}