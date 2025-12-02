using System.Collections.ObjectModel;
using Gestión_Hotelera.Data.Repositories;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.ViewModel
{
    public class HabitacionesListaViewModel : ViewModelBaseSeguro
    {
        private readonly HabitacionRepository _habRepo;
        private readonly TipoHabitacionRepository _tipoRepo;
        private readonly HotelRepository _hotelRepo;

        public ObservableCollection<HabitacionListItem> Habitaciones { get; set; }

        public HabitacionesListaViewModel()
        {
            if (IsInDesignMode)
            {
                Habitaciones = new ObservableCollection<HabitacionListItem>();
                return;
            }

            _habRepo = new HabitacionRepository();
            _hotelRepo = new HotelRepository();
            _tipoRepo = new TipoHabitacionRepository();

            CargarHabitaciones();
        }

        private void CargarHabitaciones()
        {
            Habitaciones = new ObservableCollection<HabitacionListItem>();

            var lista = _habRepo.GetAll();

            foreach (var h in lista)
            {
                var hotel = _hotelRepo.GetById(h.HotelId);
                var tipo = _tipoRepo.GetByHotelAndTipo(h.HotelId, h.TipoId);

                Habitaciones.Add(new HabitacionListItem
                {
                    HabitacionId = h.HabitacionId,
                    Numero = h.NumeroHabitacion,
                    Piso = h.Piso,
                    Estado = h.Estado,
                    HotelNombre = hotel?.Nombre ?? "(Sin hotel)",
                    TipoNombre = tipo?.NombreTipo ?? "(Sin tipo)",
                    PrecioNoche = tipo?.PrecioNoche ?? 0
                });
            }
        }
    }
}