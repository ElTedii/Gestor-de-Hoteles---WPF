using System;
using System.Diagnostics;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.Services
{
    public class FacturaPrinter
    {
        public string GenerarPDF(FacturaModel factura, string clienteNombre, string hotelNombre)
        {
            string carpeta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FacturasKuma");
            Directory.CreateDirectory(carpeta);

            string ruta = Path.Combine(carpeta, $"Factura_{factura.FacturaId}.pdf");

            Document doc = new Document(PageSize.A4);
            PdfWriter.GetInstance(doc, new FileStream(ruta, FileMode.Create));

            doc.Open();

            var titulo = new Paragraph("FACTURA - KUMA HOTEL", new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD));
            titulo.Alignment = Element.ALIGN_CENTER;

            doc.Add(titulo);
            doc.Add(new Paragraph("\n"));

            doc.Add(new Paragraph($"Factura: {factura.FacturaId}"));
            doc.Add(new Paragraph($"Fecha emisión: {factura.FechaEmision:dd/MM/yyyy}"));
            doc.Add(new Paragraph($"Hotel: {hotelNombre}"));
            doc.Add(new Paragraph($"Cliente: {clienteNombre}"));
            doc.Add(new Paragraph("\n---------------------------------------\n"));

            doc.Add(new Paragraph($"Hospedaje: {factura.MontoHospedaje:C}"));
            doc.Add(new Paragraph($"Servicios: {factura.MontoServicios:C}"));
            doc.Add(new Paragraph($"TOTAL: {factura.Total:C}"));
            doc.Add(new Paragraph("\n---------------------------------------\n"));

            doc.Add(new Paragraph("Gracias por su preferencia."));
            doc.Close();

            return ruta;
        }
    }
}