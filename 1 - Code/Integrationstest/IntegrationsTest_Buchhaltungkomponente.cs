using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using Common.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using Util.Common.DataTypes;
using Util.Common.Interfaces;
using Util.PersistenceServices.Implementations;
using Util.PersistenceServices.Interfaces;
using Util.TimeServices;

namespace Test.Integrationtest
{
    [TestClass]
    public class IntegrationsTest_Buchhaltungskomponente
    {
        private static IPersistenceServices persistenceService = null;
        private static ITransactionServices transactionService = null;

        private static IBuchhaltungServices buchhaltungService = null;
        private static IUnterbeauftragungServicesFuerBuchhaltung unterbeauftragungServiceFuerBuchhaltung = null;
        private static IUnterbeauftragungServices unterbeauftragungServices = null;

        private static FrachtauftragDTO fauf1DTO; 
        private static FrachtauftragDTO fauf2DTO;
        private static FrachtauftragDTO fauf3DTO;

        private static Gutschrift g1;

        private static FrachtabrechnungDTO fab1DTO;
        private static FrachtabrechnungDTO fab2DTO;
        private static FrachtabrechnungDTO fab3DTO;

        private static FrachtfuehrerDTO ffDTO;
        private static FrachtfuehrerRahmenvertragDTO ffRvDTO;

        private static FrachtfuehrerRahmenvertragDTO frv_hh_bhv;

        [ClassInitialize]
        public static void InitializeTests(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();

            PersistenceServicesFactory.CreateSimpleMySQLPersistenceService(out persistenceService, out transactionService);

            var timeServicesMock = new Mock<ITimeServices>();

            Mock<IFrachtfuehrerServicesFürUnterbeauftragung> frachtfuehrerServicesFürUnterbeauftragung = new Mock<IFrachtfuehrerServicesFürUnterbeauftragung>();
            unterbeauftragungServices = new UnterbeauftragungKomponenteFacade(persistenceService, transactionService, frachtfuehrerServicesFürUnterbeauftragung.Object);
            unterbeauftragungServiceFuerBuchhaltung = unterbeauftragungServices as IUnterbeauftragungServicesFuerBuchhaltung;
            buchhaltungService = new BuchhaltungKomponenteFacade(
            persistenceService,
            transactionService,
            unterbeauftragungServiceFuerBuchhaltung
            );

            Mock<FrachtfuehrerRahmenvertragDTO> ffRvDTOMock = new Mock<FrachtfuehrerRahmenvertragDTO>();
            Mock<FrachtfuehrerRahmenvertrag> ffRv = new Mock<FrachtfuehrerRahmenvertrag>();

            ffRvDTO = new FrachtfuehrerRahmenvertragDTO();
            ffDTO = new FrachtfuehrerDTO();
            ffRvDTO.Frachtfuehrer = ffDTO;

            FrachtfuehrerDTO frfHH_BHV = new FrachtfuehrerDTO();
            unterbeauftragungServices.CreateFrachtfuehrer(ref frfHH_BHV);
            frv_hh_bhv = new FrachtfuehrerRahmenvertragDTO();
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
            frv_hh_bhv.Frachtfuehrer = frfHH_BHV;
            frv_hh_bhv.Zeitvorgabe = TimeSpan.Parse("2"); // 2 Tage
            unterbeauftragungServices.CreateFrachtfuehrerRahmenvertrag(ref frv_hh_bhv);

            fauf1DTO = new FrachtauftragDTO() { Dokument = null, FrachtfuehrerRahmenvertrag = frv_hh_bhv, PlanEndezeit = new DateTime(), PlanStartzeit = new DateTime(), Status = FrachtauftragStatusTyp.NichtAbgeschlossen, VerwendeteKapazitaetFEU = 5, VerwendeteKapazitaetTEU = 10 };
            fauf2DTO = new FrachtauftragDTO() { FraNr = 2, Dokument = null, FrachtfuehrerRahmenvertrag = new FrachtfuehrerRahmenvertragDTO(), PlanEndezeit = new DateTime(), PlanStartzeit = new DateTime(), Status = FrachtauftragStatusTyp.NichtAbgeschlossen, VerwendeteKapazitaetFEU = 5, VerwendeteKapazitaetTEU = 10 };
            fauf3DTO = new FrachtauftragDTO() { FraNr = 3, Dokument = null, FrachtfuehrerRahmenvertrag = new FrachtfuehrerRahmenvertragDTO(), PlanEndezeit = new DateTime(), PlanStartzeit = new DateTime(), Status = FrachtauftragStatusTyp.NichtAbgeschlossen, VerwendeteKapazitaetFEU = 5, VerwendeteKapazitaetTEU = 10 };
            unterbeauftragungServices.CreateFrachtauftrag(ref fauf1DTO);

            g1 = new Gutschrift() { Betrag = new WaehrungsType(3), Kontodaten = new KontodatenType("DE00210501700012345678", "RZTIAT22263") };

            fab1DTO = new FrachtabrechnungDTO() { Gutschrift = g1, FaufNr = fauf1DTO.FraNr, IstBestaetigt = true, Rechnungsbetrag = new WaehrungsType(30), RechnungsNr = 1 };
            fab2DTO = new FrachtabrechnungDTO() { Gutschrift = new Gutschrift(), FaufNr = fauf2DTO.FraNr, IstBestaetigt = true, Rechnungsbetrag = new WaehrungsType(40), RechnungsNr = 2 };
            fab3DTO = new FrachtabrechnungDTO() { Gutschrift = new Gutschrift(), FaufNr = fauf3DTO.FraNr, IstBestaetigt = true, Rechnungsbetrag = new WaehrungsType(50), RechnungsNr = 3 };
        }

        [ClassCleanup]
        public static void CleanUpClass()
        {
        }

        [TestMethod, TestCategory("IntegrationsTest")]
        public void TestErstelleFrachtabrechnungUndBezhaleSieSuccess()
        {
            buchhaltungService.CreateFrachtabrechnung(fauf1DTO.FraNr);
            buchhaltungService.PayFrachtabrechnung(ref fab1DTO);
        }

        [TestCleanup]
        public void CleanUpTest()
        {
        }
    }
}
