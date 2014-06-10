using ApplicationCore.AuftragKomponente.AccessLayer;
using ApplicationCore.AuftragKomponente.DataAccessLayer;
using ApplicationCore.BankAdapter.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.FrachtfuehrerAdapter.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using ApplicationCore.PDFErzeugungsKomponente.AccesLayer;
using ApplicationCore.TransportnetzKomponente.AccessLayer;
using ApplicationCore.TransportnetzKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.AccessLayer;
using ApplicationCore.TransportplanungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using Common.DataTypes;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using Util.Common.DataTypes;
using Util.MailServices.Implementations;
using Util.MailServices.Interfaces;
using Util.PersistenceServices.Implementations;
using Util.PersistenceServices.Interfaces;
using Util.TimeServices;

namespace Tests.Integrationtest
{
    [TestClass]
    [DeploymentItem("Configurations/ConnectionStrings.config", "Configurations")]
    [DeploymentItem("Mysql.Data.dll")]
    public class IntegrationsTest_PdfUndMail
    {
        private static IPersistenceServices persistenceService = null;
        private static ITransactionServices transactionService = null;

        private static IBuchhaltungServices bhs = null;
        private static IBuchhaltungsServicesFuerBank bhsfb = null;
        private static IBankAdapterServicesFuerBuchhaltung basfbh = null;
        private static ITransportplanungServices tps = null;
        private static IAuftragServices aufs = null;
        private static IUnterbeauftragungServices ubs = null;
        private static IFrachtfuehrerServicesFürUnterbeauftragung ffsfub = null;
        private static IGeschaeftspartnerServices gps = null;
        private static IPDFErzeugungsServicesFuerBuchhaltung pdfsfb = null;
        private static IPDFErzeugungsServicesFuerUnterbeauftragung pdfsfub = null;
        private static IMailServices mails = null;
        private static ITransportnetzServices tpns = null;

        private static LokationDTO hamburgLokation;
        private static LokationDTO bremerhavenLokation;
        private static LokationDTO shanghaiLokation;
        private static TransportbeziehungDTO hh_bhv;
        private static TransportbeziehungDTO bhv_sh;

        [ClassInitialize]
        public static void InitializeTests(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();

            PersistenceServicesFactory.CreateSimpleMySQLPersistenceService(out persistenceService, out transactionService);

            var timeServicesMock = new Mock<ITimeServices>();
            //// Wir müssen einen fixen Zeitpunkt simulieren, ansonsten sind bei der Ausführung/Planung evtl. die Verträge oder Angebote abgelaufen
            timeServicesMock.Setup(ts => ts.Now)
               .Returns(DateTime.Parse("31.08.2013 12:00"));

            pdfsfb = new PDFErzeugungKomponenteFacade();
            pdfsfub = new PDFErzeugungKomponenteFacade();
            mails = MailServicesFactory.CreateMailServices();
            basfbh = new BankAdapterFacade();
            IBuchhaltungServicesFuerFrachtfuehrerAdapter bhsfff = bhs as IBuchhaltungServicesFuerFrachtfuehrerAdapter;
            ffsfub = new FrachtfuehrerAdapterFacade(ref bhsfff);
            tpns = new TransportnetzKomponenteFacade();
            gps = new GeschaeftspartnerKomponenteFacade(persistenceService, transactionService);
            ubs = new UnterbeauftragungKomponenteFacade(
                persistenceService,
                transactionService,
                ffsfub,
                gps,
                pdfsfub,
                mails);
            aufs = new AuftragKomponenteFacade(persistenceService, transactionService, timeServicesMock.Object, mails, ubs as IUnterbeauftragungServicesFuerAuftrag);
            IAuftragServicesFürTransportplanung aufsftp = aufs as IAuftragServicesFürTransportplanung;
            tps = new TransportplanungKomponenteFacade(
                persistenceService,
                transactionService,
                aufsftp,
                ubs as IUnterbeauftragungServicesFürTransportplanung,
                tpns as ITransportnetzServicesFürTransportplanung,
                timeServicesMock.Object);
            aufsftp.RegisterTransportplanungServiceFürAuftrag(tps as ITransportplanungServicesFürAuftrag);
            bhs = new BuchhaltungKomponenteFacade(
                persistenceService,
                transactionService,
                basfbh,
                tps as ITransportplanServicesFuerBuchhaltung,
                aufs as IAuftragServicesFuerBuchhaltung,
                gps,
                pdfsfb,
                mails,
                tpns);

            bhs.SetzeUnterbeauftragungServices(ubs as IUnterbeauftragungServicesFuerBuchhaltung);
            bhsfb = bhs as IBuchhaltungsServicesFuerBank;

            ////Bis hier hin alles gut 

            hamburgLokation = new LokationDTO("Hamburg", TimeSpan.Parse("10"), 10);
            bremerhavenLokation = new LokationDTO("Bremerhaven", TimeSpan.Parse("15"), 15);
            shanghaiLokation = new LokationDTO("Shanghai", TimeSpan.Parse("10"), 10);

            tpns.CreateLokation(ref hamburgLokation);
            tpns.CreateLokation(ref bremerhavenLokation);
            tpns.CreateLokation(ref shanghaiLokation);

            hh_bhv = new TransportbeziehungDTO(hamburgLokation, bremerhavenLokation);
            bhv_sh = new TransportbeziehungDTO(bremerhavenLokation, shanghaiLokation);

            tpns.CreateTransportbeziehung(ref hh_bhv);
            tpns.CreateTransportbeziehung(ref bhv_sh);

            FrachtfuehrerDTO frfHH_BHV = new FrachtfuehrerDTO();
            ubs.CreateFrachtfuehrer(ref frfHH_BHV);
            FrachtfuehrerRahmenvertragDTO frv_hh_bhv = new FrachtfuehrerRahmenvertragDTO();
            frv_hh_bhv.GueltigkeitAb = DateTime.Parse("01.01.2013");
            frv_hh_bhv.GueltigkeitBis = DateTime.Parse("31.12.2013");
            frv_hh_bhv.Abfahrtszeiten = new System.Collections.Generic.List<StartzeitDTO>() 
            { 
                new StartzeitDTO() { Wochentag = DayOfWeek.Tuesday, Uhrzeit = 8 },
                new StartzeitDTO() { Wochentag = DayOfWeek.Wednesday, Uhrzeit = 8 },
                new StartzeitDTO() { Wochentag = DayOfWeek.Friday, Uhrzeit = 8 }
            };
            frv_hh_bhv.KapazitaetTEU = 2;
            frv_hh_bhv.KostenFix = 1000;
            frv_hh_bhv.KostenProTEU = 100;
            frv_hh_bhv.KostenProFEU = 200;
            frv_hh_bhv.FuerTransportAufTransportbeziehung = hh_bhv.TbNr;
            frv_hh_bhv.Frachtfuehrer = frfHH_BHV;
            frv_hh_bhv.Zeitvorgabe = TimeSpan.Parse("2"); // 2 Tage
            ubs.CreateFrachtfuehrerRahmenvertrag(ref frv_hh_bhv);

            FrachtfuehrerDTO frfBHV = new FrachtfuehrerDTO();
            ubs.CreateFrachtfuehrer(ref frfBHV);
            FrachtfuehrerRahmenvertragDTO frv_bhv_sh = new FrachtfuehrerRahmenvertragDTO();
            frv_bhv_sh.GueltigkeitAb = DateTime.Parse("01.01.2013");
            frv_bhv_sh.GueltigkeitBis = DateTime.Parse("31.12.2013");
            frv_bhv_sh.Abfahrtszeiten = new System.Collections.Generic.List<StartzeitDTO>() 
            { 
                new StartzeitDTO() { Wochentag = DayOfWeek.Monday, Uhrzeit = 8 },
                new StartzeitDTO() { Wochentag = DayOfWeek.Thursday, Uhrzeit = 10 },
                new StartzeitDTO() { Wochentag = DayOfWeek.Saturday, Uhrzeit = 8 }
            };
            frv_bhv_sh.KapazitaetTEU = 4;
            frv_bhv_sh.KostenFix = 2000;
            frv_bhv_sh.KostenProTEU = 200;
            frv_bhv_sh.KostenProFEU = 400;
            frv_bhv_sh.FuerTransportAufTransportbeziehung = bhv_sh.TbNr;
            frv_bhv_sh.Frachtfuehrer = frfBHV;
            frv_bhv_sh.Zeitvorgabe = TimeSpan.Parse("5"); // 5 Tage
            ubs.CreateFrachtfuehrerRahmenvertrag(ref frv_bhv_sh);
        }
    }
}

//        [TestMethod, TestCategory("IntegrationsTest")]
//        public void TestErstelleKundenrechnungUndVerchickeUndBezahleSuccess()
//        {
//            string user = Interaction.InputBox("Bitte geben Sie einen Benutzername ein", "Benutzername");
//            user = user.Trim();
//            string password = Interaction.InputBox("Bitte geben Sie das Passwort ein", "Passwort");
//            password = password.Trim();
//            NetworkCredential nc = new NetworkCredential(user, password);
//            mails.SetCredentials(nc);
//            GeschaeftspartnerDTO gpDTO = new GeschaeftspartnerDTO() { Vorname = "Harmut", Nachname = "Hunt", Email = new EMailType("hartmut.hunt@ganzschlechterkunde.de") };
//            AdresseDTO adrDTO = new AdresseDTO() { Land = "Trinidad & Tobago", Wohnort = "Hinter der Mühle", PLZ = "12345", Strasse = "Kaputte Straße", Hausnummer = "2" };
//            gpDTO.Adressen.Add(adrDTO);
//            gps.CreateGeschaeftspartner(ref gpDTO);
//            SendungsanfrageDTO saDTO = new SendungsanfrageDTO();

//            SendungspositionDTO sp1 = new SendungspositionDTO();
//            saDTO.Sendungspositionen.Add(sp1);
//            saDTO.AbholzeitfensterStart = DateTime.Parse("01.09.2013");
//            saDTO.AbholzeitfensterEnde = DateTime.Parse("10.09.2013");
//            saDTO.AngebotGültigBis = DateTime.Now.AddHours(1);
//            saDTO.StartLokation = hamburgLokation.LokNr;
//            saDTO.ZielLokation = shanghaiLokation.LokNr;
//            saDTO.AuftrageberNr = gpDTO.GpNr;

//            aufs.CreateSendungsanfrage(ref saDTO);
//            aufs.PlaneSendungsanfrage(saDTO.SaNr);
//            List<TransportplanDTO> pläne = tps.FindTransportplaeneZuSendungsanfrage(saDTO.SaNr);
//            Assert.IsTrue(pläne.Count >= 1);

//            TransportplanDTO planÜberBhv = pläne.Find((plan) =>
//            {
//                return plan.TransportplanSchritte.Find((tpa) =>
//                {
//                    TransportAktivitaetDTO ta = tpa as TransportAktivitaetDTO;
//                    if (ta != null)
//                    {
//                        return ta.FuerTransportAufTransportbeziehung == hh_bhv.TbNr;
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                }) != null;
//            });
//            Assert.IsTrue(planÜberBhv != null);

//            Assert.IsTrue(planÜberBhv.TransportplanSchritte.Count == 2);
//            pläne = tps.FindTransportplaeneZuSendungsanfrage(saDTO.SaNr);
//            Assert.IsTrue(pläne.Count == 1);
//            Assert.IsTrue(pläne[0].TpNr == planÜberBhv.TpNr);

//            foreach (TransportplanDTO tpDTO in pläne)
//            {
//                Sendungsanfrage sa = aufs.FindSendungsanfrage(tpDTO.SaNr).ToEntity();
//                aufs.NimmAngebotAn(sa.SaNr);
//                tps.FühreTransportplanAus(tpDTO.TpNr);
//            }

//            KundenrechnungDTO krdto = bhs.ErstelleKundenrechnung(1, 1);
//            Assert.IsTrue(krdto.RechnungsNr > 0);

//            ZahlungseingangDTO zeDTO = new ZahlungseingangDTO() { KrNr = 1, Zahlungsbetrag = new WaehrungsType(50000) };
//            bhsfb.VerarbeiteZahlungseingang(ref zeDTO);

//            SendungsanfrageDTO saDTO_erg = aufs.FindSendungsanfrage(krdto.Sendungsanfrage);
//            Assert.IsTrue(saDTO_erg.Status == SendungsanfrageStatusTyp.Abgeschlossen);
//        }
//    }
//}
