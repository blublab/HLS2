using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ApplicationCore.AuftragKomponente.AccessLayer;
using ApplicationCore.AuftragKomponente.DataAccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using ApplicationCore.PDFErzeugungsKomponente.AccesLayer;
using ApplicationCore.TransportnetzKomponente.AccessLayer;
using ApplicationCore.TransportnetzKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using Common.DataTypes;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Util.MailServices.Implementations;
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
               pdfFacade as IPDFErzeugungsServicesFuerUnterbeauftragung,
               new Mock<IMailServices>().Object);
           auftragServices = new AuftragKomponenteFacade(
               persistenceService,
               transactionService,
               new TimeServices(),
               null,
               unterbeauftragungsServices);
           transportnetzServices = new TransportnetzKomponenteFacade();

           var auftragsServicesFürTransportplanung = auftragServices as IAuftragServicesFürTransportplanung;
           Mock<IFrachtfuehrerServicesFürUnterbeauftragung> frachtfuehrerServicesMock = null;

            ITransportplanungServices transportplanungsServices = new TransportplanungKomponenteFacade(
               persistenceService,
               transactionService,
               auftragsServicesFürTransportplanung,
               unterbeauftragungsServices as IUnterbeauftragungServicesFürTransportplanung,
               transportnetzServices as ITransportnetzServicesFürTransportplanung,
               new TimeServices());
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
 
        private void CreateTestdata()
        {
            log.Debug("Creating testdata.");
            var gpDTOs = new GeschaeftspartnerDTO[]
            {
                new GeschaeftspartnerDTO()
                {
                    Nachname = "Mustermann",
                    Vorname = "Max",
                    Email = new EMailType("max.mustermann@haw-hamburg.de"),
                    Adressen = new AdresseDTO[]
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
                    Adressen = new AdresseDTO[]
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
            CreateSendungsanfragen(gpDTOs);
        }

        private void CreateSendungsanfragen(GeschaeftspartnerDTO[] gpDTO)
        {
            LokationDTO hamburgLokation = new LokationDTO("Hamburg", TimeSpan.Parse("10"), 10);
            LokationDTO bremerhavenLokation = new LokationDTO("Bremerhaven", TimeSpan.Parse("15"), 15);
            LokationDTO shanghaiLokation = new LokationDTO("Shanghai", TimeSpan.Parse("10"), 10);

            transportnetzServices.CreateLokation(ref hamburgLokation);
            transportnetzServices.CreateLokation(ref bremerhavenLokation);
            transportnetzServices.CreateLokation(ref shanghaiLokation);

            SendungsanfrageDTO saDTO = new SendungsanfrageDTO();
            saDTO.Sendungspositionen.Add(new SendungspositionDTO());
            saDTO.AbholzeitfensterStart = DateTime.Parse("29.07.2013");
            saDTO.AbholzeitfensterEnde = DateTime.Parse("04.08.2013");
            saDTO.StartLokation = hamburgLokation.LokNr;
            saDTO.ZielLokation = bremerhavenLokation.LokNr;
            saDTO.AuftrageberNr = gpDTO[0].GpNr;
            auftragServices.CreateSendungsanfrage(ref saDTO);

            saDTO = new SendungsanfrageDTO();
            saDTO.Sendungspositionen.Add(new SendungspositionDTO());
            saDTO.AbholzeitfensterStart = DateTime.Parse("24.07.2014");
            saDTO.AbholzeitfensterEnde = DateTime.Parse("09.08.2014");
            saDTO.StartLokation = shanghaiLokation.LokNr;
            saDTO.ZielLokation = hamburgLokation.LokNr;
            saDTO.AuftrageberNr = gpDTO[1].GpNr;
            auftragServices.CreateSendungsanfrage(ref saDTO);
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