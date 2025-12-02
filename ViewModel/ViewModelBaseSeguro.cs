using System.ComponentModel;

namespace Gestión_Hotelera.ViewModel
{
    public class ViewModelBaseSeguro : ViewModelBase
    {
        public bool IsInDesignMode =>
            DesignerProperties.GetIsInDesignMode(new System.Windows.DependencyObject());
    }
}