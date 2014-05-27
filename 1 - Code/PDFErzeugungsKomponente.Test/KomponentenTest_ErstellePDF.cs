using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using ApplicationCore.PDFErzeugungsKomponente.AccesLayer;
using ApplicationCore.TransportplanungKomponente.DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using Util.Common.DataTypes;

namespace Test.KomponentenTest.PDFErzeugungsKomponente
{
    [TestClass]
    public class KomponentenTest_PDFErzeugungsKomponente
    {
        private static KundenrechnungDTO krDTO = null;

        /// <summary>
        /// Initialize the PDFErzeugungs
        /// <param name="testContext">Testcontext provided by framework</param>
        /// </summary>
        [ClassInitialize]
        public static void InitializePDFErzeugung(TestContext testContext)
        {
            krDTO = new KundenrechnungDTO();
            krDTO.RechnungsNr = 123;
            krDTO.RechnungBezahlt = false;
            ////krDTO.Zahlungseingang = new Zahlungseingang();
            ////krDTO.Sendungsanfrage = 1;
            ////krDTO.Rechnungsadressen = new List<int>();
        }

        [TestMethod]
        public void TestPDFErzeugung()
        {
            PDFErzeugungKomponenteFacade pdf = new PDFErzeugungKomponenteFacade();
            IList<TransportplanSchrittDTO> tpSchritte = new List<TransportplanSchrittDTO>();
            tpSchritte.Add(new TransportplanSchrittDTO() { Kosten = 50 });
            tpSchritte.Add(new TransportplanSchrittDTO() { Kosten = 22 });
            decimal kosten = 0;
            foreach (TransportplanSchrittDTO tpsDTO in tpSchritte)
            {
                kosten += tpsDTO.Kosten;
            }
            krDTO.Rechnungsbetrag = new WaehrungsType(kosten);
            GeschaeftspartnerDTO gpDTO = new GeschaeftspartnerDTO();
            Adresse kundenadresse = new Adresse() { Strasse = "ABC-Strasse", Hausnummer = "123", Land = "Nimmerland", PLZ = "xyz", Wohnort = "hinterm Baum" };
            gpDTO.Adressen.Add(kundenadresse.ToDTO());

            pdf.ErstelleKundenrechnungPDF(krDTO, tpSchritte, gpDTO);
            ////Assert.IsTrue(File.Exists(file), "Datei existiert nicht");

            ////Adresse kundenadresse = new Adresse() { Strasse = "ABC-Strasse", Hausnummer = "123", Land = "Nimmerland", PLZ = "xyz", Wohnort = "hinterm Baum" }; 
            ////WaehrungsType betrag = new WaehrungsType(987654321);
            ////KundenrechnungDTO krDTO = new KundenrechnungDTO() { RechnungsNr = 1, RechnungBezahlt = false, Rechnungsadresse = 1, Rechnungsbetrag = betrag, Sendungsanfrage = 1 };
            ////IPDFErzeugungsServicesFuerBuchhaltung pDFErzeungServicesFuerBuchhaltung = new PDFErzeugungKomponenteFacade();
            ////pDFErzeungServicesFuerBuchhaltung.ErstelleKundenrechnungPDF(ref krDTO, 
        }
    }
}
