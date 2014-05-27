using ApplicationCore.AuftragKomponente.AccessLayer;
using ApplicationCore.AuftragKomponente.DataAccessLayer;
using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.AccessLayer;
using ApplicationCore.TransportplanungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using Common.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Util.Common.DataTypes;
using Util.MailServices.Interfaces;
using Util.PersistenceServices.Implementations;
using Util.PersistenceServices.Interfaces;

namespace BuchhaltungKomponente.Test
{
    [TestClass]
    public class KomponentenTest_BuchhaltungKomponente
    {
        private static IPersistenceServices persistenceService = null;
        private static ITransactionServices transactionService = null;
        private static IBuchhaltungServices buchhaltungsService = null;
        private static IBuchhaltungsServicesFuerBank buchhaltungsServiceFuerBank = null;
        private static Mock<IUnterbeauftragungServicesFuerBuchhaltung> unterbeauftragungServiceMock = null;
        private static Mock<ITransportplanServicesFuerBuchhaltung> transportplanMock = null;
        private static Mock<IAuftragServicesFuerBuchhaltung> auftragMock = null;
        private static Mock<IGeschaeftspartnerServices> geschaeftspartnerMock = null;
        private static Mock<IPDFErzeugungsServicesFuerBuchhaltung> pdfMock = null;
        private static Mock<IMailServices> mailMock = null;

        /// <summary>
        /// Initialize the persistence
        /// </summary>
        /// <param name="testContext">Testcontext provided by framework</param>
        [ClassInitialize]
        public static void InitializePersistence(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();

            PersistenceServicesFactory.CreateSimpleMySQLPersistenceService(out persistenceService, out transactionService);
            unterbeauftragungServiceMock = new Mock<IUnterbeauftragungServicesFuerBuchhaltung>();
            transportplanMock = new Mock<ITransportplanServicesFuerBuchhaltung>();
            auftragMock = new Mock<IAuftragServicesFuerBuchhaltung>();
            geschaeftspartnerMock = new Mock<IGeschaeftspartnerServices>();
            pdfMock = new Mock<IPDFErzeugungsServicesFuerBuchhaltung>();
            mailMock = new Mock<IMailServices>();
            buchhaltungsService = new BuchhaltungKomponenteFacade(
                persistenceService,
                transactionService,
                new Mock<IBankAdapterServicesFuerBuchhaltung>().Object,
                transportplanMock.Object,
                auftragMock.Object,
                geschaeftspartnerMock.Object,
                pdfMock.Object,
                mailMock.Object);
            buchhaltungsService.SetzeUnterbeauftragungServices(unterbeauftragungServiceMock.Object as IUnterbeauftragungServicesFuerBuchhaltung);
            buchhaltungsServiceFuerBank = new BuchhaltungKomponenteFacade(
                persistenceService,
                transactionService,
                new Mock<IBankAdapterServicesFuerBuchhaltung>().Object,
                transportplanMock.Object,
                auftragMock.Object,
                geschaeftspartnerMock.Object,
                pdfMock.Object,
                new Mock<IMailServices>().Object);
            buchhaltungsService.SetzeUnterbeauftragungServices(unterbeauftragungServiceMock.Object as IUnterbeauftragungServicesFuerBuchhaltung);
        }

        [TestMethod]
        [ExpectedException(typeof(FrachtauftragNichtGefundenException))]
        public void TestCreateFrachtabrechnungFail()
        {
            unterbeauftragungServiceMock.Setup(unterbeauftragungService => unterbeauftragungService.PruefeObFrachtauftragVorhanden(1)).Returns(false);
            buchhaltungsService.CreateFrachtabrechnung(1);
        }

        [TestMethod]
        public void TestCreateFrachtabrechnungSuccess()
        {
            int faufNr = 1;
            unterbeauftragungServiceMock.Setup(IUnterbeauftragungServices => IUnterbeauftragungServices.PruefeObFrachtauftragVorhanden(faufNr)).Returns(true);
            FrachtabrechnungDTO fabDTO1 = buchhaltungsService.CreateFrachtabrechnung(faufNr);
            unterbeauftragungServiceMock.Verify(IUnterbeauftragungServices => IUnterbeauftragungServices.PruefeObFrachtauftragVorhanden(faufNr));
            int fabNr = fabDTO1.FabNr;
            FrachtabrechnungDTO faufDTO2 = buchhaltungsService.ReadFrachtabrechnungByID(fabNr);
            Assert.IsTrue(fabDTO1.FabNr == faufDTO2.FabNr);
        }

        [TestMethod]
        public void TestPayFrachtabrechnungSuccess()
        {
            Frachtabrechnung fab = new Frachtabrechnung() { Rechnungsbetrag = new WaehrungsType(50), IstBestaetigt = true };
            FrachtabrechnungDTO fabDTO = fab.ToDTO();
            unterbeauftragungServiceMock.Setup(IUnterbeauftragungServices => IUnterbeauftragungServices.SchliesseFrachtauftragAb(fab.FaufNr));
            (buchhaltungsService as IBuchhaltungServicesFuerFrachtfuehrerAdapter).PayFrachtabrechnung(ref fabDTO);
            unterbeauftragungServiceMock.Verify(IUnterbeauftragungServices => IUnterbeauftragungServices.SchliesseFrachtauftragAb(fab.FaufNr));
            fab = fabDTO.ToEntity();

            //Assert.IsTrue(fab.Gutschrift != null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void TestDeleteFrachtabrechnungSuccess()
        {
            int faufNr = 1;
            unterbeauftragungServiceMock.Setup(IUnterbeauftragungServices => IUnterbeauftragungServices.PruefeObFrachtauftragVorhanden(faufNr)).Returns(true);
            FrachtabrechnungDTO fabDTO1 = buchhaltungsService.CreateFrachtabrechnung(faufNr);
            unterbeauftragungServiceMock.Verify(IUnterbeauftragungServices => IUnterbeauftragungServices.PruefeObFrachtauftragVorhanden(faufNr));

            buchhaltungsService.DeleteFrachtabrechnung(ref fabDTO1);

            FrachtabrechnungDTO fabDTO2 = buchhaltungsService.ReadFrachtabrechnungByID(fabDTO1.FabNr);
        }

        [TestMethod]
        public void TestErstelleKundenrechnungSuccess()
        {
            transportplanMock.Setup(ITransportplanServicesFuerBuchhaltung => ITransportplanServicesFuerBuchhaltung.FindeTransportplanUeberTpNr(1)).Returns(new TransportplanDTO());
            auftragMock.Setup(IAuftragServicesFuerBuchhaltung => IAuftragServicesFuerBuchhaltung.FindeSendungsanfrageUeberSaNr(1)).Returns(new SendungsanfrageDTO());
            geschaeftspartnerMock.Setup(IGeschaeftspartnerServices => IGeschaeftspartnerServices.FindGeschaeftspartner(It.IsAny<int>())).Returns(new GeschaeftspartnerDTO());
            pdfMock.Setup(IPDFErzeugungsServicesFuerBuchhaltung => IPDFErzeugungsServicesFuerBuchhaltung.ErstelleKundenrechnungPDF(It.IsAny<KundenrechnungDTO>(), It.IsAny<IList<TransportplanSchrittDTO>>(), It.IsAny<GeschaeftspartnerDTO>()));
            KundenrechnungDTO krDTO = buchhaltungsService.ErstelleKundenrechnung(1, 1);
            transportplanMock.Verify(ITransportplanServicesFuerBuchhaltung => ITransportplanServicesFuerBuchhaltung.FindeTransportplanUeberTpNr(1));
            auftragMock.Verify(IAuftragServicesFuerBuchhaltung => IAuftragServicesFuerBuchhaltung.FindeSendungsanfrageUeberSaNr(1));
            geschaeftspartnerMock.Verify(IGeschaeftspartnerServices => IGeschaeftspartnerServices.FindGeschaeftspartner(It.IsAny<int>()));
            pdfMock.Verify(IPDFErzeugungsServicesFuerBuchhaltung => IPDFErzeugungsServicesFuerBuchhaltung.ErstelleKundenrechnungPDF(It.IsAny<KundenrechnungDTO>(), It.IsAny<IList<TransportplanSchrittDTO>>(), It.IsAny<GeschaeftspartnerDTO>()));
            Assert.IsTrue(krDTO.RechnungsNr > 0);
        }

        [TestMethod]
        public void TestVerarbeiteZahlungseingangUndBezahleSieSuccess()
        {
            transportplanMock.Setup(ITransportplanServicesFuerBuchhaltung => ITransportplanServicesFuerBuchhaltung.FindeTransportplanUeberTpNr(1)).Returns(new TransportplanDTO());
            auftragMock.Setup(IAuftragServicesFuerBuchhaltung => IAuftragServicesFuerBuchhaltung.FindeSendungsanfrageUeberSaNr(1)).Returns(new SendungsanfrageDTO());
            geschaeftspartnerMock.Setup(IGeschaeftspartnerServices => IGeschaeftspartnerServices.FindGeschaeftspartner(It.IsAny<int>())).Returns(new GeschaeftspartnerDTO());
            KundenrechnungDTO krDTO = buchhaltungsService.ErstelleKundenrechnung(1, 1);
            Assert.IsTrue(krDTO.RechnungsNr > 0);
            krDTO.Rechnungsbetrag = new WaehrungsType(50);
            ZahlungseingangDTO zeDTO = new ZahlungseingangDTO() { Zahlungsbetrag = new WaehrungsType(50), KrNr = krDTO.RechnungsNr };
            auftragMock.Setup(IAuftragServicesFuerBuchhaltung => IAuftragServicesFuerBuchhaltung.SchliesseSendungsanfrageAb(1));
            krDTO = buchhaltungsServiceFuerBank.VerarbeiteZahlungseingang(ref zeDTO);
            auftragMock.Verify(IAuftragServicesFuerBuchhaltung => IAuftragServicesFuerBuchhaltung.SchliesseSendungsanfrageAb(1));
            
            Assert.IsTrue(krDTO.RechnungBezahlt == true);
        }
    }
}
