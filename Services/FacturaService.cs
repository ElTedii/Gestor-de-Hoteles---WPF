using Gestión_Hotelera.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;

namespace Gestión_Hotelera.Services
{
    public class FacturaService
    {
        private const string OutputFolder = "Output/Facturas";

        public FacturaService()
        {
            if (!Directory.Exists(OutputFolder))
                Directory.CreateDirectory(OutputFolder);
        }

        // =============================
        // GENERAR XML
        // =============================
        public string GenerarFacturaXML(FacturaModel f)
        {
            string xmlPath = $"{OutputFolder}/Factura_{f.FolioFactura}.xml";

            var xml = new XElement("Factura",
                new XAttribute("Folio", f.FolioFactura),
                new XElement("ClienteId", f.ClienteId),
                new XElement("HotelId", f.HotelId),
                new XElement("ReservaId", f.ReservaId),
                new XElement("EstanciaId", f.EstanciaId),
                new XElement("FechaEmision", f.FechaEmision),
                new XElement("Periodo",
                    new XElement("Entrada", f.FechaEntrada),
                    new XElement("Salida", f.FechaSalida)
                ),
                new XElement("Cobros",
                    new XElement("Anticipo", f.Anticipo),
                    new XElement("MontoHospedaje", f.MontoHospedaje),
                    new XElement("MontoServicios", f.MontoServicios),
                    new XElement("Descuento", f.Descuento),
                    new XElement("Total", f.TotalFactura)
                )
            );

            xml.Save(xmlPath);
            return xmlPath;
        }

        public string GenerarFacturaPDF(FacturaModel f)
        {
            string pdfPath = $"{OutputFolder}/Factura_{f.FolioFactura}.pdf";

            PdfDocument doc = new PdfDocument();
            doc.Info.Title = $"Factura {f.FolioFactura}";

            PdfPage page = doc.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont titleFont = new XFont("Verdana", 20, XFontStyle.Bold);
            XFont textFont = new XFont("Verdana", 12);

            int y = 40;

            // Título
            gfx.DrawString("FACTURA DE HOSPEDAJE", titleFont, XBrushes.DarkBlue, new XPoint(40, y));
            y += 40;

            // Datos principales
            gfx.DrawString($"Folio: {f.FolioFactura}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Cliente: {f.ClienteId}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Hotel: {f.HotelId}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Reserva: {f.ReservaId}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Estancia: {f.EstanciaId}", textFont, XBrushes.Black, new XPoint(40, y)); y += 30;

            gfx.DrawString($"Entrada: {f.FechaEntrada:dd/MM/yyyy}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Salida:  {f.FechaSalida:dd/MM/yyyy}", textFont, XBrushes.Black, new XPoint(40, y)); y += 30;

            // Cobros
            gfx.DrawString($"Anticipo:           ${f.Anticipo}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Hospedaje:         ${f.MontoHospedaje}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Servicios:          ${f.MontoServicios}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"Descuento:          ${f.Descuento}", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;

            gfx.DrawString("----------------------------", textFont, XBrushes.Black, new XPoint(40, y)); y += 20;
            gfx.DrawString($"TOTAL A PAGAR:   ${f.TotalFactura}", titleFont, XBrushes.DarkRed, new XPoint(40, y));

            doc.Save(pdfPath);
            return pdfPath;
        }

        public (string pdf, string xml) GenerarFactura(FacturaModel f)
        {
            string xmlPath = GenerarFacturaXML(f);
            string pdfPath = GenerarFacturaPDF(f);

            return (pdfPath, xmlPath);
        }
    }
}
