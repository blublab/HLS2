using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using Common.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using Util.PersistenceServices.Implementations;
using Util.PersistenceServices.Interfaces;

namespace Tests.KomponentenTest.GeschaeftspartnerKomponente
{
    [TestClass]
    public class KomponentenTest_GeschaeftspartnerKomponente
    {
        private static IPersistenceServices persistenceService = null;
        private static ITransactionServices transactionService = null;
        private static IGeschaeftspartnerServices geschaeftspartnerServices = null;

        /// <summary>
        /// Initialize the persistence
        /// </summary>
        /// <param name="testContext">Testcontext provided by framework</param>
        [ClassInitialize]
        public static void InitializePersistence(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();

            PersistenceServicesFactory.CreateSimpleMySQLPersistenceService(out persistenceService, out transactionService);

            geschaeftspartnerServices = new GeschaeftspartnerKomponenteFacade(persistenceService, transactionService);
        }

        [TestMethod]
        public void TestCreateGeschaeftspartnerSuccess()
        {
            GeschaeftspartnerDTO gpDTO = new GeschaeftspartnerDTO() { Vorname = "Max", Nachname = "Mustermann", EMail = new EMailType("sarstedt@acm.org")  };
            Assert.IsTrue(gpDTO.GpNr == 0, "Id of Geschaeftspartner must be null.");
            geschaeftspartnerServices.CreateGeschaeftspartner(ref gpDTO);
            Assert.IsTrue(gpDTO.GpNr > 0, "Id of Geschaeftspartner must not be 0.");
            Assert.IsTrue(gpDTO.Version > 0, "Version of Geschaeftspartner must not be 0.");
        }

        [TestMethod]
        public void TestUpdateGeschaeftspartnerSuccess()
        {
            GeschaeftspartnerDTO gpDTO = new GeschaeftspartnerDTO() { Vorname = "Max", Nachname = "Mustermann", EMail = new EMailType("sarstedt@acm.org") };
            geschaeftspartnerServices.CreateGeschaeftspartner(ref gpDTO);

            gpDTO.Vorname = "Maria";
            geschaeftspartnerServices.UpdateGeschaeftspartner(ref gpDTO);

            gpDTO = geschaeftspartnerServices.FindGeschaeftspartner(gpDTO.GpNr);
            Assert.IsTrue(gpDTO != null, "Geschaeftspartner nicht gefunden.");
            Assert.IsTrue(gpDTO.Vorname == "Maria", "Geschaeftspartner nicht geändert.");
            Assert.IsTrue(gpDTO.Nachname == "Mustermann", "Geschaeftspartner nicht geändert.");
        }

        [TestMethod]
        public void TestFindGeschaeftspartnerByIdSuccess()
        {
            GeschaeftspartnerDTO gpDTO1 = new GeschaeftspartnerDTO() { Vorname = "Heinz", Nachname = "Schmidt", EMail = new EMailType("sarstedt@acm.org") };
            geschaeftspartnerServices.CreateGeschaeftspartner(ref gpDTO1);
            GeschaeftspartnerDTO gpDTO2 = geschaeftspartnerServices.FindGeschaeftspartner(gpDTO1.GpNr);
            Assert.IsTrue(gpDTO1 == gpDTO2, "Geschaeftspartner must be the same.");      
        }

        [TestMethod]
        [ExpectedException(typeof(Util.PersistenceServices.Exceptions.OptimisticConcurrencyException))]
        public void TestUpdateGeschaeftspartnerFailWegenOptimisticConcurrencyException()
        {
            GeschaeftspartnerDTO gpDTOOriginal = new GeschaeftspartnerDTO() { Vorname = "Maria", Nachname = "Mustermann", EMail = new EMailType("sarstedt@acm.org") };
            geschaeftspartnerServices.CreateGeschaeftspartner(ref gpDTOOriginal);
            Assert.IsTrue(gpDTOOriginal.GpNr > 0, "Id of Geschaeftspartner must not be 0.");

            AutoResetEvent syncEvent1 = new AutoResetEvent(false);
            AutoResetEvent syncEvent2 = new AutoResetEvent(false);
            Task task1 = Task.Factory.StartNew(() =>
            {
                GeschaeftspartnerDTO gpDTOTask = geschaeftspartnerServices.FindGeschaeftspartner(gpDTOOriginal.GpNr);
                gpDTOTask.Nachname = "Musterfrau";
                geschaeftspartnerServices.UpdateGeschaeftspartner(ref gpDTOTask);
                syncEvent1.WaitOne();
                syncEvent2.Set();
            });

            GeschaeftspartnerDTO gpDTO = geschaeftspartnerServices.FindGeschaeftspartner(gpDTOOriginal.GpNr);
            gpDTO.Vorname = "Maria Janine";
            syncEvent1.Set();
            syncEvent2.WaitOne();
            geschaeftspartnerServices.UpdateGeschaeftspartner(ref gpDTO);

            task1.Wait();
        }
    }
}
