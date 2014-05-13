using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.PersistenceServices.Interfaces;
using Util.PersistenceServices.Implementations;
using ApplicationCore.BankAdapter.AccessLayer;
using ApplicationCore.FrachtfuehrerAdapter.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;

namespace Client.HLS_Konsole
{
    public class Program
    {
        private static IPersistenceServices persistenceServices = null;
        private static ITransactionServices transactionServices = null;

        // BuchhaltungKomponente
        private static IBuchhaltungServicesFuerFrachtfuehrerAdapter bhsfffa= null;

        //BankAdapter
        private static IBankAdapterServicesFuerBuchhaltung basfbh = null;
        private static BankAdapterFacade baf = null;

        //FrachtfueherAdapter
        private static FrachtfuehrerAdapterFacade ffaf = null;

        //UnterbeauftragungKomponente
        private static IUnterbeauftragungServicesFuerBuchhaltung ubsfbh = null;

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing...");
            init();
            Console.WriteLine("Initializing complete");

            Console.WriteLine("Filling DB with data...");
            fillDB();
            Console.WriteLine("Filling DB done.");
            
            Console.WriteLine("Start reading from FrachtabrechnungQueue: \n");
            ffaf.StarteEmpfangVonFrachtabrechnungen();
        }

        private static void init()
        {
            PersistenceServicesFactory.CreateSimpleMySQLPersistenceService(out persistenceServices, out transactionServices);

            baf = new BankAdapterFacade();
            bhsfffa = new BuchhaltungKomponenteFacade(persistenceServices, transactionServices, baf);
            ffaf = new FrachtfuehrerAdapterFacade(bhsfffa);
            ubsfbh = new UnterbeauftragungKomponenteFacade(persistenceServices, transactionServices, ffaf as IFrachtfuehrerServicesFürUnterbeauftragung);
            bhsfffa.SetzeUnterbeauftragungServices(ubsfbh);
        }

        private static void fillDB()
        {
            IUnterbeauftragungServices unterbeauftragungServices = new UnterbeauftragungKomponenteFacade(persistenceServices, transactionServices, ffaf);
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
