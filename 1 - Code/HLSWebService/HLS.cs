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
            return rechnungen;
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
                            Land = "Deutschland",
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
                            Land = "Deutschland",
                            PLZ = "10020",
                            Wohnort = "Berlin",
                            Strasse = "Schusterweg"
                        }
                    }
                },

                new GeschaeftspartnerDTO()
                {
                    Nachname = "Sarstedt",
                    Vorname = "Stefan",
                    Email = new EMailType("stefan.sarstedt@haw-hamburg.de"),
                    Adressen = new[]
                    {
                        new AdresseDTO()
                        {
                            Hausnummer = "7",
                            Land = "Deutschland",
                            PLZ = "22099",
                            Wohnort = "Hamburg",
                            Strasse = "Berliner Tor"
                        }
                    }
                },

                new GeschaeftspartnerDTO()
                {
                    Nachname = "江",
                    Vorname = "泽民",
                    Email = new EMailType("zemin.jiang@prc.cn"),
                    Adressen = new[]
                    {
                        new AdresseDTO()
                        {
                            Hausnummer = "10",
                            Land = "中国",
                            PLZ = "102300",
                            Wohnort = "北京市",
                            Strasse = "西安门"
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
            rechnungen = CreateKundenrechnungen(gpDTOs, saDTOs);
        }

        private IList<KundenrechnungDTO> rechnungen;
 
        private SendungsanfrageDTO[] CreateSendungsanfragen(GeschaeftspartnerDTO[] gpDTO)
        {
            var loks = new Dictionary<string, LokationDTO>()
            {
                { "Hamburg",        new LokationDTO("Hamburg", TimeSpan.Parse("10"), 10)        },
                { "Bremerhaven",    new LokationDTO("Bremerhaven", TimeSpan.Parse("15"), 15)    },
                { "Shanghai",       new LokationDTO("Shanghai", TimeSpan.Parse("10"), 10)       },
                { "Hongkong",       new LokationDTO("Hongkong", TimeSpan.Parse("20"), 12)       },
                { "Tokio",          new LokationDTO("Tokio", TimeSpan.Parse("15"), 15)          },
                { "Rotterdam",      new LokationDTO("Rotterdam", TimeSpan.Parse("15"), 14)      },
                { "Singapur",       new LokationDTO("Singapur", TimeSpan.Parse("8"), 18)        }  
            };
            foreach (var key in new List<string>(loks.Keys))
            {
                var dto = loks[key];
                transportnetzServices.CreateLokation(ref dto);
                loks[key] = dto;
            }

            var saDTO = new SendungsanfrageDTO();
            saDTO.Sendungspositionen.Add(new SendungspositionDTO());
            saDTO.AbholzeitfensterStart = DateTime.Parse("29.06.2014");
            saDTO.AbholzeitfensterEnde = DateTime.Parse("04.07.2014");
            saDTO.AngebotGültigBis = new DateTime(2014, 8, 4, 14, 0, 0);
            saDTO.StartLokation = loks["Hamburg"].LokNr;
            saDTO.ZielLokation = loks["Bremerhaven"].LokNr;
            saDTO.AuftrageberNr = gpDTO[0].GpNr;
            auftragServices.CreateSendungsanfrage(ref saDTO);

            var saDTO2 = new SendungsanfrageDTO();
            saDTO2.Sendungspositionen.Add(new SendungspositionDTO());
            saDTO2.AbholzeitfensterStart = DateTime.Parse("24.07.2014");
            saDTO2.AbholzeitfensterEnde = DateTime.Parse("09.08.2014");
            saDTO2.StartLokation = loks["Shanghai"].LokNr;
            saDTO2.ZielLokation = loks["Hamburg"].LokNr;
            saDTO2.AuftrageberNr = gpDTO[1].GpNr;
            auftragServices.CreateSendungsanfrage(ref saDTO2);

            var saDTO3 = new SendungsanfrageDTO();
            saDTO3.Sendungspositionen.Add(new SendungspositionDTO());
            saDTO3.AbholzeitfensterStart = DateTime.Parse("20.07.2014");
            saDTO3.AbholzeitfensterEnde = DateTime.Parse("20.08.2014");
            saDTO3.StartLokation = loks["Rotterdam"].LokNr;
            saDTO3.ZielLokation = loks["Hongkong"].LokNr;
            saDTO3.AuftrageberNr = gpDTO[2].GpNr;
            auftragServices.CreateSendungsanfrage(ref saDTO3);

            var saDTO4 = new SendungsanfrageDTO();
            saDTO4.Sendungspositionen.Add(new SendungspositionDTO());
            saDTO4.AbholzeitfensterStart = DateTime.Parse("26.07.2014");
            saDTO4.AbholzeitfensterEnde = DateTime.Parse("30.08.2014");
            saDTO4.StartLokation = loks["Singapur"].LokNr;
            saDTO4.ZielLokation = loks["Tokio"].LokNr;
            saDTO4.AuftrageberNr = gpDTO[3].GpNr;
            auftragServices.CreateSendungsanfrage(ref saDTO4);

            return new[] { saDTO, saDTO2, saDTO3, saDTO4 };
        }

        public KundenrechnungDTO[] CreateKundenrechnungen(GeschaeftspartnerDTO[] gpDTO, SendungsanfrageDTO[] saDTO)
        {
            var krDTOs = new[]
            {
                new KundenrechnungDTO()
                {
                    Rechnungsbetrag = new WaehrungsType(1200m),
                    RechnungBezahlt = true,
                    Sendungsanfrage = saDTO[0].SaNr,
                    Rechnungsadresse = gpDTO[0].Adressen[0].Id,
                    RechnungsNr = 1
                },
                new KundenrechnungDTO()
                {
                    Rechnungsbetrag = new WaehrungsType(4500m),
                    RechnungBezahlt = false,
                    Sendungsanfrage = saDTO[1].SaNr,
                    Rechnungsadresse = gpDTO[1].Adressen[0].Id,
                    RechnungsNr = 2
                },
                new KundenrechnungDTO()
                {
                    Rechnungsbetrag = new WaehrungsType(1000m),
                    RechnungBezahlt = false,
                    Sendungsanfrage = saDTO[2].SaNr,
                    Rechnungsadresse = gpDTO[2].Adressen[0].Id,
                    RechnungsNr = 3
                },
                new KundenrechnungDTO()
                {
                    Rechnungsbetrag = new WaehrungsType(3800m),
                    RechnungBezahlt = false,
                    Sendungsanfrage = saDTO[3].SaNr,
                    Rechnungsadresse = gpDTO[3].Adressen[0].Id,
                    RechnungsNr = 4
                }
            };

            for (int i = 0; i < krDTOs.Length; i++)
            {
                KundenrechnungDTO krDTO = krDTOs[i];

//                buchhaltungServices.CreateKundenrechnung(ref krDTO);
            }

            return krDTOs;
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