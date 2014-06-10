using ApplicationCore.AuftragKomponente.AccessLayer;
using ApplicationCore.BankAdapter.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.FrachtfuehrerAdapter.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.PDFErzeugungsKomponente.AccesLayer;
using ApplicationCore.TransportnetzKomponente.AccessLayer;
using ApplicationCore.TransportplanungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.MailServices.Implementations;
using Util.MailServices.Interfaces;
using Util.PersistenceServices.Implementations;
using Util.PersistenceServices.Interfaces;
using Util.TimeServices;

namespace Client.HLS_Konsole
{
    public class Program
    {
        private static IPersistenceServices persistenceServices = null;
        private static ITransactionServices transactionServices = null;
        private static IAuftragServices auftragsServices = null;
        private static IUnterbeauftragungServices unterbeauftragungsServices = null;
        private static Mock<IFrachtfuehrerServicesFürUnterbeauftragung> frachtfuehrerServicesMock = null;
        private static ITransportnetzServices transportnetzServices = null;

        private static IBuchhaltungServicesFuerFrachtfuehrerAdapter bhsfffa = null;
        private static ITransportplanServicesFuerBuchhaltung tpsfbh = null;

        private static BankAdapterFacade baf = null;
        
        private static FrachtfuehrerAdapterFacade ffaf = null;

        private static IUnterbeauftragungServicesFuerBuchhaltung ubsfbh = null;
        private static IGeschaeftspartnerServices gps = null;
        private static PDFErzeugungKomponenteFacade pdfFacade = null;
        private static IMailServices mailService = null;

        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");
            Init();
            Console.WriteLine("Initializing complete");

            Console.WriteLine("Filling DB with data...");
            PopulateDB();
            Console.WriteLine("Filling DB done.");
            
            Console.WriteLine("Start reading from FrachtabrechnungQueue: \n");
            ffaf.StarteEmpfangVonFrachtabrechnungen();
        }

        private static void Init()
        {
            PersistenceServicesFactory.CreateSimpleMySQLPersistenceService(out persistenceServices, out transactionServices);
            ITimeServices timeServices = new TimeServices();

            mailService = MailServicesFactory.CreateMailServices();
            baf = new BankAdapterFacade();
            pdfFacade = new PDFErzeugungKomponenteFacade();
            gps = new GeschaeftspartnerKomponenteFacade(persistenceServices, transactionServices);
            unterbeauftragungsServices = new UnterbeauftragungKomponenteFacade(persistenceServices, transactionServices, frachtfuehrerServicesMock.Object, gps, pdfFacade as IPDFErzeugungsServicesFuerUnterbeauftragung, mailService);
            auftragsServices = new AuftragKomponenteFacade(persistenceServices, transactionServices, timeServices, mailService, unterbeauftragungsServices as IUnterbeauftragungServicesFuerAuftrag);
            IAuftragServicesFürTransportplanung auftragsServicesFürTransportplanung = auftragsServices as IAuftragServicesFürTransportplanung;

            transportnetzServices = new TransportnetzKomponenteFacade();

            tpsfbh = new TransportplanungKomponenteFacade(
                persistenceServices,
                transactionServices,
                auftragsServicesFürTransportplanung,
                unterbeauftragungsServices as IUnterbeauftragungServicesFürTransportplanung,
                transportnetzServices as ITransportnetzServicesFürTransportplanung,
                timeServices);

            bhsfffa = new BuchhaltungKomponenteFacade(
                persistenceServices,
                transactionServices,
                baf,
                tpsfbh,
                new Mock<IAuftragServicesFuerBuchhaltung>().Object,
                new Mock<IGeschaeftspartnerServices>().Object,
                new Mock<IPDFErzeugungsServicesFuerBuchhaltung>().Object,
                new Mock<IMailServices>().Object,
                new Mock<ITransportnetzServices>().Object);
            ffaf = new FrachtfuehrerAdapterFacade(ref bhsfffa);
            ubsfbh = new UnterbeauftragungKomponenteFacade(persistenceServices, transactionServices, ffaf as IFrachtfuehrerServicesFürUnterbeauftragung, gps, pdfFacade as IPDFErzeugungsServicesFuerUnterbeauftragung, mailService);
            bhsfffa.SetzeUnterbeauftragungServices(ubsfbh);
        }

        private static void PopulateDB()
        {
            IUnterbeauftragungServices unterbeauftragungServices = new UnterbeauftragungKomponenteFacade(persistenceServices, transactionServices, ffaf, gps, pdfFacade as IPDFErzeugungsServicesFuerUnterbeauftragung, mailService);
            FrachtfuehrerRahmenvertragDTO frv_hh_bhv;
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

            FrachtauftragDTO fauf1DTO = new FrachtauftragDTO() { Dokument = null, FrachtfuehrerRahmenvertrag = frv_hh_bhv, PlanEndezeit = new DateTime(), PlanStartzeit = new DateTime(), Status = FrachtauftragStatusTyp.NichtAbgeschlossen, VerwendeteKapazitaetFEU = 5, VerwendeteKapazitaetTEU = 10 };
            FrachtauftragDTO fauf2DTO = new FrachtauftragDTO() { Dokument = null, FrachtfuehrerRahmenvertrag = frv_hh_bhv, PlanEndezeit = new DateTime(), PlanStartzeit = new DateTime(), Status = FrachtauftragStatusTyp.NichtAbgeschlossen, VerwendeteKapazitaetFEU = 5, VerwendeteKapazitaetTEU = 10 };
            FrachtauftragDTO fauf3DTO = new FrachtauftragDTO() { Dokument = null, FrachtfuehrerRahmenvertrag = frv_hh_bhv, PlanEndezeit = new DateTime(), PlanStartzeit = new DateTime(), Status = FrachtauftragStatusTyp.NichtAbgeschlossen, VerwendeteKapazitaetFEU = 5, VerwendeteKapazitaetTEU = 10 };
            unterbeauftragungServices.CreateFrachtauftrag(ref fauf1DTO);
            unterbeauftragungServices.CreateFrachtauftrag(ref fauf2DTO);
            unterbeauftragungServices.CreateFrachtauftrag(ref fauf3DTO);
        }
    }
}
