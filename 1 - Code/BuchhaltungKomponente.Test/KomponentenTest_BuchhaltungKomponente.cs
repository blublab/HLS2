using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using Common.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Util.Common.DataTypes;
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
        private static Mock<IUnterbeauftragungServicesFuerBuchhaltung> unterbeauftragungServiceMock = null;

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
            buchhaltungsService = new BuchhaltungKomponenteFacade(
                persistenceService,
                transactionService,
                new Mock<IBankAdapterServicesFuerBuchhaltung>().Object);
        }

        [TestMethod]
        [ExpectedException(typeof(FrachtauftragNichtGefundenException))]
        public void TestCreateFrachtabrechnungFail()
        {
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
            //buchhaltungsService.PayFrachtabrechnung(ref fabDTO);
            unterbeauftragungServiceMock.Setup(IUnterbeauftragungServices => IUnterbeauftragungServices.SchliesseFrachtauftragAb(fab.FaufNr));
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
    }
}
