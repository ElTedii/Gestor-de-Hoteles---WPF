using Gestión_Hotelera.Model;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Gestión_Hotelera.ViewModel
{
    public class FacturaDetalleViewModel : ViewModelBase
    {
        private readonly FacturaModel _factura;
        private readonly ClienteModel _cliente;
        private readonly HotelModel _hotel;
        private readonly string _rutaPDF;

        public string ClienteNombre => _cliente?.NombreCompleto ?? "—";
        public string HotelNombre => _hotel?.Nombre ?? "—";
        public string TotalTexto => _factura != null ? _factura.Total.ToString("C") : "$0.00";

        public ICommand AbrirPDFCommand { get; }
        public ICommand GuardarPDFCommand { get; }

        public FacturaDetalleViewModel(
            FacturaModel factura,
            ClienteModel cliente,
            HotelModel hotel,
            string rutaPDF)
        {
            _factura = factura;
            _cliente = cliente;
            _hotel = hotel;
            _rutaPDF = rutaPDF;

            AbrirPDFCommand = new ViewModelCommand(ExecuteAbrirPDF);
            GuardarPDFCommand = new ViewModelCommand(ExecuteGuardarPDF);
        }

        private void ExecuteAbrirPDF(object obj)
        {
            if (!File.Exists(_rutaPDF))
            {
                MessageBox.Show("El archivo PDF no existe.");
                return;
            }

            try
            {
                System.Diagnostics.Process.Start(_rutaPDF);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo abrir el PDF.\n" + ex.Message);
            }
        }

        private void ExecuteGuardarPDF(object obj)
        {
            if (!File.Exists(_rutaPDF))
            {
                MessageBox.Show("El archivo PDF no existe.");
                return;
            }

            try
            {
                string destino = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    $"Factura_{_factura.FacturaId}.pdf");

                File.Copy(_rutaPDF, destino, overwrite: true);
                MessageBox.Show("PDF copiado al Escritorio.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo guardar el PDF.\n" + ex.Message);
            }
        }
    }
}