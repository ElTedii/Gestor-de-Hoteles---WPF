using iTextSharp.text;
using iTextSharp.text.pdf;
using Gestión_Hotelera.Model;
using System;
using System.IO;

namespace Gestión_Hotelera.Services
{
    public class FacturaPrinter
    {
        private static readonly Font fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
        private static readonly Font fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11, BaseColor.BLACK);
        private static readonly Font fontTitleWhite = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 22, BaseColor.WHITE);

        // ============================================================
        // GENERAR FACTURA PDF
        // ============================================================
        public string GenerarFacturaPDF(FacturaModel factura, ClienteModel cliente, HotelModel hotel)
        {
            string carpeta = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "KumaHotel_Facturas");

            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            string ruta = Path.Combine(carpeta, $"Factura_{factura.FacturaId}.pdf");

            using (var fs = new FileStream(ruta, FileMode.Create))
            {
                var doc = new Document(PageSize.LETTER, 40, 40, 40, 40);
                var writer = PdfWriter.GetInstance(doc, fs);

                doc.Open();

                // ============================================================
                // ENCABEZADO PROFESIONAL CON LOGO
                // ============================================================
                PdfPTable encabezado = new PdfPTable(2);
                encabezado.WidthPercentage = 100;
                encabezado.SetWidths(new float[] { 1f, 2f });

                // LOGO
                var logo = CargarLogo();
                PdfPCell celdaLogo;

                if (logo != null)
                {
                    logo.ScaleAbsolute(70, 70);
                    celdaLogo = new PdfPCell(logo)
                    {
                        Border = Rectangle.NO_BORDER,
                        HorizontalAlignment = Element.ALIGN_LEFT,
                        PaddingBottom = 10
                    };
                }
                else
                {
                    celdaLogo = new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER };
                }

                encabezado.AddCell(celdaLogo);

                // TITULO
                var celdaTitulo = new PdfPCell(new Phrase("FACTURA DE HOSPEDAJE", fontTitleWhite))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    PaddingRight = 10,
                    PaddingBottom = 10,
                    BackgroundColor = new BaseColor(13, 27, 75)
                };

                encabezado.AddCell(celdaTitulo);

                doc.Add(encabezado);
                doc.Add(new Paragraph("\n"));

                // ============================================================
                // INFORMACIÓN DEL HOTEL / CLIENTE
                // ============================================================
                PdfPTable tablaInfo = new PdfPTable(2);
                tablaInfo.WidthPercentage = 100;
                tablaInfo.SetWidths(new float[] { 1f, 2f });

                AgregarFilaInfo(tablaInfo, "Hotel", hotel.Nombre);
                AgregarFilaInfo(tablaInfo, "Dirección", $"{hotel.Ciudad}, {hotel.Estado}, {hotel.Pais}");
                AgregarFilaInfo(tablaInfo, "Cliente", cliente.NombreCompleto);
                AgregarFilaInfo(tablaInfo, "Correo", cliente.Correo);
                AgregarFilaInfo(tablaInfo, "Folio factura", factura.FacturaId.ToString());
                AgregarFilaInfo(tablaInfo, "Fecha emisión", factura.FechaEmision.ToString("dd/MM/yyyy HH:mm"));

                doc.Add(tablaInfo);
                doc.Add(new Paragraph("\n"));

                // ============================================================
                // TABLA DE DETALLE
                // ============================================================
                PdfPTable tabla = new PdfPTable(2);
                tabla.WidthPercentage = 100;
                tabla.SetWidths(new float[] { 2f, 1f });

                // ENCABEZADOS
                tabla.AddCell(CeldaEncabezado("Concepto"));
                tabla.AddCell(CeldaEncabezado("Importe"));

                tabla.AddCell(CeldaCampo("Hospedaje"));
                tabla.AddCell(CeldaPrecio(factura.MontoHospedaje));

                tabla.AddCell(CeldaCampo("Servicios adicionales"));
                tabla.AddCell(CeldaPrecio(factura.MontoServicios));

                doc.Add(tabla);
                doc.Add(new Paragraph("\n"));

                // ============================================================
                // TOTAL
                // ============================================================
                PdfPTable tablaTotal = new PdfPTable(2);
                tablaTotal.WidthPercentage = 100;
                tablaTotal.SetWidths(new float[] { 2f, 1f });

                tablaTotal.AddCell(new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER });

                var celdaTotal = new PdfPCell(new Phrase($"TOTAL    ${factura.Total:N2}", fontTitleWhite))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    BackgroundColor = new BaseColor(240, 240, 255),
                    Padding = 10
                };

                tablaTotal.AddCell(celdaTotal);
                doc.Add(tablaTotal);

                // ============================================================
                // PIE DE PÁGINA
                // ============================================================
                doc.Add(new Paragraph("\n"));
                doc.Add(new Paragraph("Gracias por su preferencia.", fontNormal));

                doc.Close();
                writer.Close();
            }

            return ruta;
        }

        // ============================================================
        // CARGAR LOGO DESDE /Resources
        // ============================================================
        private static iTextSharp.text.Image CargarLogo()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Resources/logo-minimal.png");
                var resourceStream = System.Windows.Application.GetResourceStream(uri);

                if (resourceStream == null)
                    return null;

                using (var ms = new MemoryStream())
                {
                    resourceStream.Stream.CopyTo(ms);
                    var img = iTextSharp.text.Image.GetInstance(ms.ToArray());

                    img.ScaleAbsolute(70, 70);
                    img.Alignment = iTextSharp.text.Image.ALIGN_LEFT;

                    return img;
                }
            }
            catch
            {
                return null;
            }
        }

        // ============================================================
        // CELDAS UTILITARIAS
        // ============================================================
        private void AgregarFilaInfo(PdfPTable tabla, string titulo, string valor)
        {
            tabla.AddCell(new PdfPCell(new Phrase(titulo, fontBold)) { Border = Rectangle.NO_BORDER });
            tabla.AddCell(new PdfPCell(new Phrase(valor, fontNormal)) { Border = Rectangle.NO_BORDER });
        }

        private PdfPCell CeldaEncabezado(string texto)
        {
            return new PdfPCell(new Phrase(texto, fontBold))
            {
                BackgroundColor = new BaseColor(235, 235, 250),
                Padding = 8
            };
        }

        private PdfPCell CeldaCampo(string texto)
        {
            return new PdfPCell(new Phrase(texto, fontNormal)) { Padding = 8 };
        }

        private PdfPCell CeldaPrecio(decimal monto)
        {
            return new PdfPCell(new Phrase($"${monto:N2}", fontNormal))
            {
                HorizontalAlignment = Element.ALIGN_RIGHT,
                Padding = 8
            };
        }
    }
}