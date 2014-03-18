using ApplicationCore.AuftragKomponente.AccessLayer;
using ApplicationCore.FrachtfuehrerAdapter.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.TransportnetzKomponente.AccessLayer;
using ApplicationCore.TransportnetzKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using log4net;
using System;
using Util.PersistenceServices.Implementations;
using Util.PersistenceServices.Interfaces;
using Util.TimeServices;

namespace HLSServerService
{
    public class ApplicationBuilder
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static ITransportnetzServices transportnetzServices = null;
        private static IUnterbeauftragungServices unterbeauftragungsServices = null;

        public static void BuildApplication()
        {
            IPersistenceServices persistenceService;
            ITransactionServices transactionService;

            PersistenceServicesFactory.CreateSimpleMySQLPersistenceService(out persistenceService, out transactionService);

            ITimeServices timeServices = new TimeServices();

            transportnetzServices = new TransportnetzKomponenteFacade();
            IGeschaeftspartnerServices geschaeftspartnerServices = new GeschaeftspartnerKomponenteFacade(persistenceService, transactionService);
            IAuftragServices auftragsServices = new AuftragKomponenteFacade(persistenceService, transactionService, timeServices);
            IAuftragServicesFürTransportplanung auftragsServicesFürTransportplanung = auftragsServices as IAuftragServicesFürTransportplanung;
            IFrachtfuehrerServicesFürUnterbeauftragung frachtfuehrerServices = new FrachtfuehrerAdapterFacade();
            unterbeauftragungsServices = new UnterbeauftragungKomponenteFacade(persistenceService, transactionService, frachtfuehrerServices);
            ITransportplanungServices transportplanungsServices = new TransportplanungKomponenteFacade(persistenceService, transactionService, auftragsServicesFürTransportplanung, unterbeauftragungsServices as IUnterbeauftragungServicesFürTransportplanung, transportnetzServices as ITransportnetzServicesFürTransportplanung, timeServices);
            auftragsServicesFürTransportplanung.RegisterTransportplanungServiceFürAuftrag(transportplanungsServices as ITransportplanungServicesFürAuftrag);
        }

        public static void CreateTestdata()
        {
            Log.Debug("Creating testdata.");

            LokationDTO hamburgLokation = new LokationDTO("Hamburg", TimeSpan.Parse("10"), 10);
            LokationDTO bremerhavenLokation = new LokationDTO("Bremerhaven", TimeSpan.Parse("15"), 15);
            LokationDTO shanghaiLokation = new LokationDTO("Shanghai", TimeSpan.Parse("10"), 10);

            transportnetzServices.CreateLokation(ref hamburgLokation);
            transportnetzServices.CreateLokation(ref bremerhavenLokation);
            transportnetzServices.CreateLokation(ref shanghaiLokation);

            TransportbeziehungDTO hh_bhv = new TransportbeziehungDTO(hamburgLokation, bremerhavenLokation);
            TransportbeziehungDTO bhv_sh = new TransportbeziehungDTO(bremerhavenLokation, shanghaiLokation);

            transportnetzServices.CreateTransportbeziehung(ref hh_bhv);
            transportnetzServices.CreateTransportbeziehung(ref bhv_sh);

            FrachtfuehrerDTO frfHH_BHV = new FrachtfuehrerDTO();
            unterbeauftragungsServices.CreateFrachtfuehrer(ref frfHH_BHV);
        }
    }
}
