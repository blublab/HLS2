using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using Common.DataTypes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using NHibernate.Type;
using Util.PersistenceServices.Implementations;
using Util.PersistenceServices.Interfaces;
using System.Collections.Generic;

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
            GeschaeftspartnerDTO gpDTO = new GeschaeftspartnerDTO() { Vorname = "Max", Nachname = "Mustermann", Email = new EMailType("max@mustermann.de") };
            Assert.IsTrue(gpDTO.GpNr == 0, "Id of Geschaeftspartner must be null.");
            geschaeftspartnerServices.CreateGeschaeftspartner(ref gpDTO);
            Assert.IsTrue(gpDTO.GpNr > 0, "Id of Geschaeftspartner must not be 0.");
            Assert.IsTrue(gpDTO.Email != null, "Email != null");
            Assert.IsTrue(gpDTO.Version > 0, "Version of Geschaeftspartner must not be 0.");
        }

        [TestMethod]
        public void TestUpdateGeschaeftspartnerSuccess()
        {
            GeschaeftspartnerDTO gpDTO = new GeschaeftspartnerDTO() { Vorname = "Max", Nachname = "Mustermann", Email = new EMailType("max@mustermann.de") };
            geschaeftspartnerServices.CreateGeschaeftspartner(ref gpDTO);

            gpDTO.Vorname = "Maria";
            geschaeftspartnerServices.UpdateGeschaeftspartner(ref gpDTO);

            gpDTO = geschaeftspartnerServices.FindGeschaeftspartner(gpDTO.GpNr);
            Assert.IsTrue(gpDTO != null, "Geschaeftspartner nicht gefunden.");
            Assert.IsTrue(gpDTO.Vorname == "Maria", "Geschaeftspartner nicht geändert.");
            Assert.IsTrue(gpDTO.Nachname == "Mustermann", "Geschaeftspartner nicht geändert.");
        }

        [TestMethod]
        public void TestFindAdresseByIdSuccess()
        {
            AdresseDTO adDTO1 = new AdresseDTO() { Strasse = "strasse1", Hausnummer = "1", PLZ = "12345", Wohnort = "Hamstadt", Land = "Deutscheland" };

            geschaeftspartnerServices.CreateAdresse(ref adDTO1);
            AdresseDTO adDTO2 = geschaeftspartnerServices.FindAdresse(adDTO1.Id);
            Assert.IsTrue(adDTO2 == adDTO1, "Adresse nach speichern nicht korrekt hergestellt.");
        }

        [TestMethod]
        public void TestFindGeschaeftspartnerByIdSuccess()
        {
            Adresse ad1 = new Adresse() { Strasse = "strasse1", Hausnummer = "1", PLZ = "12345", Wohnort = "Hamstadt", Land = "Deutscheland" };
            GeschaeftspartnerDTO gpDTO1 = new GeschaeftspartnerDTO() { Vorname = "Heinz", Nachname = "Schmidt", Email = new EMailType("heinz@schmidt.de"), Adressen = new List<AdresseDTO>() };
            gpDTO1.Adressen.Add(ad1.ToDTO());
            geschaeftspartnerServices.CreateGeschaeftspartner(ref gpDTO1);
            GeschaeftspartnerDTO gpDTO2 = geschaeftspartnerServices.FindGeschaeftspartner(gpDTO1.GpNr);
            Assert.IsTrue(gpDTO1.GpNr == gpDTO2.GpNr, "Geschaeftspartner must be the same.");      
        }

        [TestMethod]
        [ExpectedException(typeof(Util.PersistenceServices.Exceptions.OptimisticConcurrencyException))]
        public void TestUpdateGeschaeftspartnerFailWegenOptimisticConcurrencyException()
        {
            GeschaeftspartnerDTO gpDTOOriginal = new GeschaeftspartnerDTO() { Vorname = "Maria", Nachname = "Mustermann", Email = new EMailType("maria@mustermann.de") };
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
