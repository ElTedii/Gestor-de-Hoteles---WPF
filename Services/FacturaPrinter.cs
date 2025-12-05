using System;
using System.IO;
using System.Windows;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Gestión_Hotelera.Model;

namespace Gestión_Hotelera.Helpers
{
    public static class FacturaPrinter
    {
        private static readonly string _folder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FacturasKuma");

        // ============================================================
        //   MÉTODO PRINCIPAL (PDF)
        // ============================================================
        public static string GenerarFacturaPDF(
            FacturaModel factura,
            ClienteModel cliente,
            HotelModel hotel,
            EstanciaActivaModel estancia,
            decimal descuento)
        {
            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            string filePath = Path.Combine(_folder, $"Factura{factura.FacturaId}.pdf");

            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                var doc = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter.GetInstance(doc, fs);
                doc.Open();

                // ---------------- LOGO ----------------
                var logo = CargarLogo();
                if (logo != null)
                {
                    logo.ScaleAbsolute(65, 65);
                    logo.Alignment = Image.ALIGN_LEFT;
                    doc.Add(logo);
                }

                // ---------------- TÍTULO ----------------
                var titulo = new Paragraph("FACTURA DE HOSPEDAJE",
                    new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, BaseColor.WHITE))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };

                PdfPTable headerBg = new PdfPTable(1);
                headerBg.WidthPercentage = 100;
                PdfPCell cellTitle = new PdfPCell(titulo)
                {
                    BackgroundColor = new BaseColor(14, 23, 55),
                    Border = Rectangle.NO_BORDER,
                    Padding = 15
                };
                headerBg.AddCell(cellTitle);
                doc.Add(headerBg);


                // ============================================================
                //   DATOS GENERALES
                // ============================================================
                PdfPTable tablaDatos = new PdfPTable(2);
                tablaDatos.WidthPercentage = 100;
                tablaDatos.SetWidths(new float[] { 2, 5 });

                AgregarFila(tablaDatos, "Hotel", hotel.Nombre);
                AgregarFila(tablaDatos, "Dirección", hotel.Domicilio);
                AgregarFila(tablaDatos, "Cliente", cliente.NombreCompleto);
                AgregarFila(tablaDatos, "Correo", cliente.Correo);

                AgregarFila(tablaDatos, "Número factura", factura.FacturaId.ToString());
                AgregarFila(tablaDatos, "Fecha emisión", factura.FechaEmision.ToLocalTime().ToString("dd/MM/yyyy HH:mm"));
                AgregarFila(tablaDatos, "Código reservación", estancia.ReservaId.ToString());

                AgregarFila(tablaDatos, "Entrada", estancia.FechaEntrada.ToString("dd/MM/yyyy"));
                AgregarFila(tablaDatos, "Salida", estancia.FechaSalida.ToString("dd/MM/yyyy"));

                AgregarFila(tablaDatos, "Habitación", estancia.NumeroHabitacion.ToString());
                AgregarFila(tablaDatos, "Precio por noche", estancia.PrecioNoche.ToString("C"));

                int noches = (int)(estancia.FechaSalida - estancia.FechaEntrada).TotalDays;
                if (noches < 1) noches = 1;

                AgregarFila(tablaDatos, "Noches", noches.ToString());

                doc.Add(tablaDatos);
                doc.Add(new Paragraph("\n"));


                // ============================================================
                //   TABLA DE IMPORTES
                // ============================================================
                PdfPTable tablaTotales = new PdfPTable(2);
                tablaTotales.WidthPercentage = 100;
                tablaTotales.SetWidths(new float[] { 4, 2 });

                AgregarEncabezado(tablaTotales, "Concepto");
                AgregarEncabezado(tablaTotales, "Importe");

                decimal hospedaje = noches * estancia.PrecioNoche;
                AgregarFila2(tablaTotales, "Hospedaje", hospedaje.ToString("C"));

                AgregarFila2(tablaTotales, "Servicios adicionales", factura.MontoServicios.ToString("C"));
                AgregarFila2(tablaTotales, "Anticipo", $"-{estancia.Anticipo:C}");

                if (descuento > 0)
                    AgregarFila2(tablaTotales, "Descuento", $"-{descuento:C}");

                doc.Add(tablaTotales);
                doc.Add(new Paragraph("\n"));


                // ============================================================
                //   TOTAL FINAL
                // ============================================================
                PdfPTable tablaTotal = new PdfPTable(2);
                tablaTotal.WidthPercentage = 100;
                tablaTotal.SetWidths(new float[] { 4, 2 });

                PdfPCell lblTotal = new PdfPCell(
                    new Phrase("TOTAL", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, new BaseColor(44, 84, 255))))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 10
                };

                PdfPCell valTotal = new PdfPCell(
                    new Phrase(factura.Total.ToString("C"),
                    new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD, new BaseColor(44, 84, 255))))
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 10
                };

                tablaTotal.AddCell(lblTotal);
                tablaTotal.AddCell(valTotal);
                doc.Add(tablaTotal);


                // ---------------- PIE ----------------
                var pie = new Paragraph("\nGracias por su preferencia.\nKuma Hotel",
                    new Font(Font.FontFamily.HELVETICA, 10, Font.ITALIC, BaseColor.DARK_GRAY))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingBefore = 25f
                };

                doc.Add(pie);
                doc.Close();
            }

            return filePath;
        }


        // ============================================================
        //   XML
        // ============================================================
        public static string GenerarFacturaXML(
            FacturaModel factura,
            ClienteModel cliente,
            HotelModel hotel,
            EstanciaActivaModel estancia,
            decimal descuento)
        {
            string filePath = Path.Combine(_folder, $"Factura{factura.FacturaId}.xml");

            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("<Factura>");

                writer.WriteLine($"  <FacturaId>{factura.FacturaId}</FacturaId>");
                writer.WriteLine($"  <FechaEmision>{factura.FechaEmision:yyyy-MM-dd HH:mm}</FechaEmision>");

                writer.WriteLine("  <Hotel>");
                writer.WriteLine($"    <Nombre>{hotel.Nombre}</Nombre>");
                writer.WriteLine($"    <Direccion>{hotel.Domicilio}</Direccion>");
                writer.WriteLine("  </Hotel>");

                writer.WriteLine("  <Cliente>");
                writer.WriteLine($"    <Nombre>{cliente.NombreCompleto}</Nombre>");
                writer.WriteLine($"    <Correo>{cliente.Correo}</Correo>");
                writer.WriteLine("  </Cliente>");

                writer.WriteLine("  <Reservacion>");
                writer.WriteLine($"    <ReservaId>{estancia.ReservaId}</ReservaId>");
                writer.WriteLine($"    <Entrada>{estancia.FechaEntrada:yyyy-MM-dd}</Entrada>");
                writer.WriteLine($"    <Salida>{estancia.FechaSalida:yyyy-MM-dd}</Salida>");
                writer.WriteLine($"    <Habitacion>{estancia.NumeroHabitacion}</Habitacion>");
                writer.WriteLine($"    <PrecioNoche>{estancia.PrecioNoche}</PrecioNoche>");
                writer.WriteLine("  </Reservacion>");

                writer.WriteLine("  <Montos>");
                writer.WriteLine($"    <Hospedaje>{factura.MontoHospedaje}</Hospedaje>");
                writer.WriteLine($"    <Servicios>{factura.MontoServicios}</Servicios>");
                writer.WriteLine($"    <Anticipo>{estancia.Anticipo}</Anticipo>");
                writer.WriteLine($"    <Descuento>{descuento}</Descuento>");
                writer.WriteLine($"    <Total>{factura.Total}</Total>");
                writer.WriteLine("  </Montos>");

                writer.WriteLine("</Factura>");
            }

            return filePath;
        }


        // ============================================================
        //   JSON
        // ============================================================
        public static string GenerarFacturaJSON(
            FacturaModel factura,
            ClienteModel cliente,
            HotelModel hotel,
            EstanciaActivaModel estancia,
            decimal descuento)
        {
            string filePath = Path.Combine(_folder, $"Factura{factura.FacturaId}.json");

            var jsonObj = new
            {
                facturaId = factura.FacturaId,
                fechaEmision = factura.FechaEmision.ToString("yyyy-MM-dd HH:mm"),

                hotel = new
                {
                    nombre = hotel.Nombre,
                    direccion = hotel.Domicilio
                },

                cliente = new
                {
                    nombre = cliente.NombreCompleto,
                    correo = cliente.Correo
                },

                reservacion = new
                {
                    reservaId = estancia.ReservaId,
                    entrada = estancia.FechaEntrada.ToString("yyyy-MM-dd"),
                    salida = estancia.FechaSalida.ToString("yyyy-MM-dd"),
                    habitacion = estancia.NumeroHabitacion,
                    precioNoche = estancia.PrecioNoche
                },

                montos = new
                {
                    hospedaje = factura.MontoHospedaje,
                    servicios = factura.MontoServicios,
                    anticipo = estancia.Anticipo,
                    descuento = descuento,
                    total = factura.Total
                }
            };

            File.WriteAllText(filePath,
                Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented));

            return filePath;
        }


        // ============================================================
        //   LOGO + HELPERS
        // ============================================================
        private static Image CargarLogo()
        {
            try
            {
                var uri = new Uri("pack://application:,,,/Resources/logo-minimal.png", UriKind.Absolute);
                var stream = Application.GetResourceStream(uri)?.Stream;
                if (stream == null) return null;

                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return Image.GetInstance(ms.ToArray());
                }
            }
            catch
            {
                return null;
            }
        }

        private static void AgregarFila(PdfPTable tabla, string etiqueta, string valor)
        {
            tabla.AddCell(new PdfPCell(new Phrase(etiqueta, new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))
            {
                BackgroundColor = new BaseColor(235, 235, 235),
                BorderWidth = 0.5f
            });

            tabla.AddCell(new PdfPCell(new Phrase(valor, new Font(Font.FontFamily.HELVETICA, 10)))
            {
                BorderWidth = 0.5f
            });
        }

        private static void AgregarEncabezado(PdfPTable tabla, string texto)
        {
            tabla.AddCell(new PdfPCell(new Phrase(texto,
                new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, new BaseColor(90, 110, 255))))
            {
                BackgroundColor = new BaseColor(240, 240, 255),
                Padding = 8,
                Border = Rectangle.NO_BORDER
            });
        }

        private static void AgregarFila2(PdfPTable tabla, string concepto, string importe)
        {
            tabla.AddCell(new PdfPCell(new Phrase(concepto, new Font(Font.FontFamily.HELVETICA, 10)))
            {
                BorderWidth = 0.2f
            });

            tabla.AddCell(new PdfPCell(new Phrase(importe,
                new Font(Font.FontFamily.HELVETICA, 10)))
            {
                BorderWidth = 0.2f,
                HorizontalAlignment = Element.ALIGN_RIGHT
            });
        }
    }
}