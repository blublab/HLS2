using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using Common.DataTypes;
using log4net;
using Moq;
using Util.Common.DataTypes;
using Util.MailServices.Interfaces;
using Util.PersistenceServices.Implementations;
using Util.PersistenceServices.Interfaces;
using Util.TimeServices;

namespace HLSWebService
{
    public class HLS
    {
        private readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IAuftragServices auftragServices;
        private readonly IGeschaeftspartnerServices geschaeftspartnerServices;
        private readonly ITransportnetzServices transportnetzServices;
        private readonly IBuchhaltungServices buchhaltungServices;
        private readonly IUnterbeauftragungServices unterbeauftragungServices;

        public HLS()
        {
            IPersistenceServices persistenceService;
            ITransactionServices transactionService;
            PersistenceServicesFactory.CreateSimpleMySQLPersistenceService(
                out persistenceService,
                out transactionService);
            geschaeftspartnerServices =
                new GeschaeftspartnerKomponenteFacade(persistenceService, transactionService);
            var pdfFacade = new PDFErzeugungKomponenteFacade();
            var gps = new GeschaeftspartnerKomponenteFacade(persistenceService, transactionService);
            var unterbeauftragungsServices = new UnterbeauftragungKomponenteFacade(
                persistenceService,
                transactionService,
                new Mock<IFrachtfuehrerServicesFürUnterbeauftragung>().Object,
                gps,
                pdfFacade,
                new Mock<IMailServices>().Object);
            auftragServices = new AuftragKomponenteFacade(
                persistenceService,
                transactionService,
                new TimeServices(),
                null,
                unterbeauftragungsServices);
            transportnetzServices = new TransportnetzKomponenteFacade();
            var auftragsServicesFürTransportplanung = auftragServices as IAuftragServicesFürTransportplanung;

            ITransportplanungServices transportplanungsServices = new TransportplanungKomponenteFacade(
               persistenceService,
               transactionService,
               auftragsServicesFürTransportplanung,
               unterbeauftragungsServices,
               transportnetzServices as ITransportnetzServicesFürTransportplanung,
               new TimeServices());
            buchhaltungServices = new BuchhaltungKomponenteFacade(
                persistenceService,
                transactionService,
                new BankAdapterFacade(),
                transportplanungsServices as ITransportplanServicesFuerBuchhaltung,
                auftragServices as IAuftragServicesFuerBuchhaltung,
                gps,
                pdfFacade, 
                new Mock<IMailServices>().Object,
                transportnetzServices);
            var bhsfff =
                buchhaltungServices as IBuchhaltungServicesFuerFrachtfuehrerAdapter;
            unterbeauftragungServices = new UnterbeauftragungKomponenteFacade(
                persistenceService,
                transactionService,
                new FrachtfuehrerAdapterFacade(ref bhsfff),
                gps,
                pdfFacade,
                new Mock<IMailServices>().Object);
            buchhaltungServices.SetzeUnterbeauftragungServices(
                unterbeauftragungServices as IUnterbeauftragungServicesFuerBuchhaltung);

            auftragsServicesFürTransportplanung.RegisterTransportplanungServiceFürAuftrag(
                transportplanungsServices as ITransportplanungServicesFürAuftrag);
            CreateTestdata();
        }

        public IList<SendungsanfrageDTO> GetSendungsanfragen(long saNr)
        {
            return saNr < 0
                ? auftragServices.GetSendungsanfragen()
                : auftragServices.GetSendungsanfragen().Where(s => s.SaNr == saNr).ToList();
        }

        public IList<KundenrechnungDTO> GetKundenrechnungen(int rechnungsNr)
        {
            return rechnungsNr < 0
                ? buchhaltungServices.GetKundenrechnungen()
                : buchhaltungServices.GetKundenrechnungen()
                .Where(s => s.RechnungsNr == rechnungsNr).ToList();
        } 

        private void CreateTestdata()
        {
            log.Debug("Creating testdata.");
            var gpDTOs = new[]
            {
                new GeschaeftspartnerDTO()
                {
                    Nachname = "Mustermann",
                    Vorname = "Max",
                    Email = new EMailType("max.mustermann@haw-hamburg.de"),
                    Adressen = new[]
                    {
                        new AdresseDTO()
                        {
                            Hausnummer = "123",
                            Land = "de",
                            PLZ = "22089",
                            Wohnort = "Hamburg",
                            Strasse = "Musterstraße"
                        }
                    }
                },

                new GeschaeftspartnerDTO()
                {
                    Nachname = "Schuster",
                    Vorname = "Franz",
                    Email = new EMailType("franz.schuster@bla.de"),
                    Adressen = new[]
                    {
                        new AdresseDTO()
                        {
                            Hausnummer = "456",
                            Land = "de",
                            PLZ = "10020",
                            Wohnort = "Berlin",
                            Strasse = "Schusterweg"
                        }
                    }
                }
            };
            for (var i = 0; i < gpDTOs.Length; i++)
            {
                geschaeftspartnerServices.CreateGeschaeftspartner(ref gpDTOs[i]);
            }
            var saDTOs = CreateSendungsanfragen(gpDTOs);

            // Kundenrechnungen erstellen.
   //         CreateKundenrechnungen(gpDTOs, saDTOs);
        }

        private SendungsanfrageDTO[] CreateSendungsanfragen(GeschaeftspartnerDTO[] gpDTO)
        {
            var hamburgLokation = new LokationDTO("Hamburg", TimeSpan.Parse("10"), 10);
            var bremerhavenLokation = new LokationDTO("Bremerhaven", TimeSpan.Parse("15"), 15);
            var shanghaiLokation = new LokationDTO("Shanghai", TimeSpan.Parse("10"), 10);

            transportnetzServices.CreateLokation(ref hamburgLokation);
            transportnetzServices.CreateLokation(ref bremerhavenLokation);
            transportnetzServices.CreateLokation(ref shanghaiLokation);

            var saDTO = new SendungsanfrageDTO();
            saDTO.Sendungspositionen.Add(new SendungspositionDTO());
            saDTO.AbholzeitfensterStart = DateTime.Parse("29.07.2013");
            saDTO.AbholzeitfensterEnde = DateTime.Parse("04.08.2013");
            saDTO.StartLokation = hamburgLokation.LokNr;
            saDTO.ZielLokation = bremerhavenLokation.LokNr;
            saDTO.AuftrageberNr = gpDTO[0].GpNr;
            auftragServices.CreateSendungsanfrage(ref saDTO);

            var saDTO2 = new SendungsanfrageDTO();
            saDTO2.Sendungspositionen.Add(new SendungspositionDTO());
            saDTO2.AbholzeitfensterStart = DateTime.Parse("24.07.2014");
            saDTO2.AbholzeitfensterEnde = DateTime.Parse("09.08.2014");
            saDTO2.StartLokation = shanghaiLokation.LokNr;
            saDTO2.ZielLokation = hamburgLokation.LokNr;
            saDTO2.AuftrageberNr = gpDTO[1].GpNr;
            auftragServices.CreateSendungsanfrage(ref saDTO2);

            return new[] { saDTO, saDTO2 };
        }

        public void CreateKundenrechnungen(GeschaeftspartnerDTO[] gpDTO, SendungsanfrageDTO[] saDTO)
        {
            var krDTOs = new[]
            {
                new KundenrechnungDTO()
                {
                    Rechnungsbetrag = new WaehrungsType(120m),
                    RechnungBezahlt = true,
                    Sendungsanfrage = 1,
                    Rechnungsadresse = 10,
                    RechnungsNr = -1
                },
                new KundenrechnungDTO()
                {
                    Rechnungsbetrag = new WaehrungsType(450m),
                    RechnungBezahlt = false,
                    Sendungsanfrage = 2,
                    Rechnungsadresse = 13,
                    RechnungsNr = -1
                }
            };

            for (int i = 0; i < krDTOs.Length; i++)
            {
                KundenrechnungDTO krDTO = krDTOs[i];
                buchhaltungServices.CreateKundenrechnung(ref krDTO);
            }
        }

        public LokationDTO FindLokation(long lokNr)
        {
            return transportnetzServices.FindLokation(lokNr);
        }

        public GeschaeftspartnerDTO FindGeschaeftspartner(int gpNr)
        {
            return geschaeftspartnerServices.FindGeschaeftspartner(gpNr);
        }
    }
}