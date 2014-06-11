using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using Common.DataTypes;
using Common.Implementations;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.DataTypes;

namespace ApplicationCore.PDFErzeugungsKomponente.AccesLayer
{
    public class PDFErzeugungKomponenteFacade : IPDFErzeugungsServicesFuerBuchhaltung, IPDFErzeugungsServicesFuerUnterbeauftragung
    {
        private static string logoPath = @"C:\Users\Yavuz\Desktop\PdfData\HLS-Logo.png";

        public PDFErzeugungKomponenteFacade()
        {
        }

        private string[] ErstelleKundenanschrift(AdresseDTO adresse)
        {
            string[] result = new string[5] 
            {
                "Straße: " + adresse.Strasse,
                "Hausnummer: " + adresse.Hausnummer,
                "PLZ: " + adresse.PLZ,
                "Wohnort: " + adresse.Wohnort,
                "Land: " + adresse.Land
            };
            return result;
        }

        private string[] ErstelleTransportplanschrittkosten(IList<TransportplanSchrittDTO> tpSchritte)
        {
            string[] result = new string[tpSchritte.Count];
            int i = 0;
            foreach (TransportplanSchrittDTO tpsDTO in tpSchritte)
            {
                decimal wert = Math.Round(tpsDTO.Kosten, 2);
                result[i] = wert + "";
                i++;
            }
            return result;
        }

        private string SpeichereKundenRechnungPDF(KundenrechnungDTO krDTO, string[] kundenadresse, string gesamtsumme, string erstellungsdatum, string[] tpSchritte, List<string> wege)
        {
            // Dokument
            PdfDocument pdfDocument = new PdfDocument();
            pdfDocument.Info.Title = @"Kundenrechnung für Rechnung Nr. " + krDTO.RechnungsNr;

            // Page
            PdfPage pdfPage = pdfDocument.AddPage();

            // XGraphics
            XGraphics gfx = XGraphics.FromPdfPage(pdfPage);

            // Create a font
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

            // Logo
            try
            {
                XImage image = XImage.FromFile(logoPath);
                double x = (1000 - (image.PixelWidth * (72 / image.HorizontalResolution))) / 2;
                ////gfx.DrawImage(image, x, 50);

                //XImage xImage = XImage.FromFile(logoPath);
                gfx.DrawImage(image, 10, 10, image.PixelWidth, image.PixelWidth);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }

            XFont header = new XFont("Verdana", 22, XFontStyle.Underline);
            gfx.DrawString("Kundenrechnung", header, XBrushes.Black, new XRect(0, 300, pdfPage.Width, pdfPage.Height), XStringFormats.TopCenter);

            // Kundenanschrift
            int y = 100;
            foreach (string str in kundenadresse)
            {
                gfx.DrawString(str, font, XBrushes.Black, new XRect(50, y, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
                y += 10;
            }

            // Erstellungsdatum
            gfx.DrawString(erstellungsdatum, font, XBrushes.Black, new XRect(200, 200, pdfPage.Width, pdfPage.Height), XStringFormats.TopCenter);

            // Transportplanschrittkosten
            XFont font3 = new XFont("Verdana", 11, XFontStyle.Italic);
            int z = 0;
            int n = 0;
            foreach (string str in tpSchritte)
            {
                string weg = "";
                if (wege.Count > 0)
                {
                    weg = wege.ElementAt(n);
                }
                gfx.DrawString(weg + " " + str + "€", font3, XBrushes.Black, new XRect(0, z, pdfPage.Width, pdfPage.Height), XStringFormats.Center);
                z += 80;
                n += 1;
            }

            //Horizontale Linie#
            int j = -220;
            for (int i = 0; i < 80; i++)
            {
                gfx.DrawString("_", font, XBrushes.Black, new XRect(j, 200, pdfPage.Width, pdfPage.Height), XStringFormats.Center);
                j += 3;
            }

            // Gesamtkosten
            XFont font2 = new XFont("Verdana", 16, XFontStyle.Bold);
            gfx.DrawString("Gesamtkosten: " + gesamtsumme.ToString() + "€", font2, XBrushes.Black, new XRect(-100, 220, pdfPage.Width, pdfPage.Height), XStringFormats.Center);

            // Speichere das Dokument
            string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Kundenrechnung.pdf";
            pdfDocument.Save(savePath);

            return savePath;
            //// Öffne das PDF mit dem default pdfViewer
            ////Process.Start(target);   
        }

        #region IPDFErzeugungsServicesFuerBuchhaltung
        public string ErstelleKundenrechnungPDF(KundenrechnungDTO krDTO, IList<TransportplanSchrittDTO> tpSchritte, GeschaeftspartnerDTO gpDTO, List<string> wege)
        {
            AdresseDTO adrDTO = gpDTO.Adressen.First<AdresseDTO>();
            WaehrungsType gesamtKosten = krDTO.Rechnungsbetrag;
            decimal gkosten = Math.Round(gesamtKosten.Wert, 2);
            DateTime erstellungsDatum = DateTime.Now;

            string[] kundenadresse = ErstelleKundenanschrift(adrDTO);
            string[] tpsStr = ErstelleTransportplanschrittkosten(tpSchritte);
            return SpeichereKundenRechnungPDF(krDTO, kundenadresse, gkosten.ToString(), erstellungsDatum.ToString(), tpsStr, wege);
        }
        #endregion

        #region IPDFErzeugungsServicesFuerUnterbeauftragung
        public string ErzeugeFrachtbriefPDF(UnterbeauftragungKomponente.DataAccessLayer.FrachtbriefDTO fbDTO)
        {
            // Dokument
            PdfDocument pdfDocument = new PdfDocument();
            pdfDocument.Info.Title = @"Frachtbrief";

            // Page
            PdfPage pdfPage = pdfDocument.AddPage();

            // XGraphics
            XGraphics gfx = XGraphics.FromPdfPage(pdfPage);

            // Create a font
            XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

            // Logo
            try
            {
                XImage image = XImage.FromFile(logoPath);
                double x = (1000 - (image.PixelWidth * (72 / image.HorizontalResolution))) / 2;
                ////gfx.DrawImage(image, x, 50);

                //XImage xImage = XImage.FromFile(logoPath);
                gfx.DrawImage(image, 10, 10, image.PixelWidth, image.PixelWidth);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex);
            }

            XFont header = new XFont("Verdana", 22, XFontStyle.Underline);
            gfx.DrawString("Frachtbrief", header, XBrushes.Black, new XRect(0, 210, pdfPage.Width, pdfPage.Height), XStringFormats.TopCenter);

            AdresseDTO hawadresse = fbDTO.AbsenderAnschrift;
            string[] hawadresseAry = new string[6] 
            {
                fbDTO.AbsenderName,
                "Straße: " + hawadresse.Strasse,
                "Hausnummer: " + hawadresse.Hausnummer,
                "PLZ: " + hawadresse.PLZ,
                "Wohnort: " + hawadresse.Wohnort,
                "Land: " + hawadresse.Land
            };
            int y = 100;
            foreach (string str in hawadresseAry)
            {
                gfx.DrawString(str, font, XBrushes.Black, new XRect(400, y, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
                y += 10;
            }

            // Erstellungsdatum
            gfx.DrawString(DateTime.Now.ToString(), font, XBrushes.Black, new XRect(200, 200, pdfPage.Width, pdfPage.Height), XStringFormats.TopCenter);
            XFont kdinfo = new XFont("Verdana", 14, XFontStyle.Italic);

            gfx.DrawString("Kundeninformationen", kdinfo, XBrushes.Black, new XRect(0, 320, pdfPage.Width, pdfPage.Height), XStringFormats.TopCenter);

            AdresseDTO kundenAdresse = fbDTO.EmpfaengerAnschrift;
            string[] kundenAdresseAry = new string[6] 
            {
                fbDTO.EmpfaengerName,
                "Straße: " + kundenAdresse.Strasse,
                "Hausnummer: " + kundenAdresse.Hausnummer,
                "PLZ: " + kundenAdresse.PLZ,
                "Wohnort: " + kundenAdresse.Wohnort,
                "Land: " + kundenAdresse.Land
            };
            int z = 350;
            foreach (string str in kundenAdresseAry)
            {
                gfx.DrawString(str, font, XBrushes.Black, new XRect(50, z, pdfPage.Width, pdfPage.Height), XStringFormats.TopLeft);
                z += 10;
            }

            // Speichere das Dokument
            string savePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Frachtbrief.pdf";
            pdfDocument.Save(savePath);

            return savePath;
        }
        #endregion
    }
}