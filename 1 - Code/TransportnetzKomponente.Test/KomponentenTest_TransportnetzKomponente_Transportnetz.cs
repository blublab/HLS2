using System;
using System.Linq;
using ApplicationCore.TransportnetzKomponente.AccessLayer;
using ApplicationCore.TransportnetzKomponente.DataAccessLayer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.KomponentenTest.TransportnetzKomponente
{
    [TestClass]
    public class KomponentenTest_TransportnetzKomponente_Transportnetz
    {
        private static ITransportnetzServices transportnetzServices = null;

        /// <summary>
        /// Initialize the persistence
        /// </summary>
        /// <param name="testContext">Testcontext provided by framework</param>
        [ClassInitialize]
        public static void InitializeTests(TestContext testContext)
        {
            log4net.Config.XmlConfigurator.Configure();

            transportnetzServices = new TransportnetzKomponenteFacade();
        }

        [ClassCleanup]
        public static void CleanUpClass()
        {
            transportnetzServices.DeleteTransportnetz("Test-.*");
        }

        [TestMethod]
        public void TestCreateLokationSuccess()
        {
            var guid = "Test-" + Guid.NewGuid();
            LokationDTO lokDto = new LokationDTO(guid, TimeSpan.FromHours(12), 100m);

            Assert.IsTrue(lokDto.LokNr < 0, "Id of Lokation must be negative.");
            transportnetzServices.CreateLokation(ref lokDto);
            Assert.IsTrue(lokDto.LokNr >= 0, "Id of Lokation must be >= 0.");
        }

        [TestMethod]
        public void TestFindLokationSuccess()
        {
            var guid = "Test-" + Guid.NewGuid();
            LokationDTO lokDto = new LokationDTO(guid, TimeSpan.FromHours(12), 100m);
            transportnetzServices.CreateLokation(ref lokDto);

            LokationDTO lokDto2 = transportnetzServices.FindLokation(guid);
            Assert.AreEqual(lokDto, lokDto2, "lokDto and lokDto2 must be equal.");
        }

        [TestMethod]
        public void TestFindLokationFail()
        {
            var guid = "Test-" + Guid.NewGuid();
            LokationDTO lokDto = transportnetzServices.FindLokation(guid);
            Assert.IsNull(lokDto, "lokDto must be null.");
        }

        [TestMethod]
        public void TestCreateTransportbeziehungSuccess()
        {
            var start = "Test-" + Guid.NewGuid();
            var ziel = "Test-" + Guid.NewGuid();
            LokationDTO lokDtoStart = new LokationDTO(start, TimeSpan.FromHours(12), 100m);
            LokationDTO lokDtoZiel = new LokationDTO(ziel, TimeSpan.FromDays(2), 80m);
            transportnetzServices.CreateLokation(ref lokDtoStart);
            transportnetzServices.CreateLokation(ref lokDtoZiel);

            TransportbeziehungDTO tbDto = new TransportbeziehungDTO(lokDtoStart, lokDtoZiel);
            Assert.IsTrue(tbDto.TbNr < 0, "Id of Transportbeziehung must be negative.");
            transportnetzServices.CreateTransportbeziehung(ref tbDto);
            Assert.IsTrue(tbDto.TbNr >= 0, "Id of Transportbeziehung must be >= 0.");
        }

        [TestMethod]
        public void TestFindTransportbeziehungSuccess()
        {
            var start = "Test-" + Guid.NewGuid();
            var ziel = "Test-" + Guid.NewGuid();
            LokationDTO lokDtoStart = new LokationDTO(start, TimeSpan.FromHours(12), 100m);
            LokationDTO lokDtoZiel = new LokationDTO(ziel, TimeSpan.FromDays(2), 80m);
            transportnetzServices.CreateLokation(ref lokDtoStart);
            transportnetzServices.CreateLokation(ref lokDtoZiel);

            TransportbeziehungDTO tbDto = new TransportbeziehungDTO(lokDtoStart, lokDtoZiel);
            transportnetzServices.CreateTransportbeziehung(ref tbDto);

            TransportbeziehungDTO tbDto2 = transportnetzServices.FindTransportbeziehung(start, ziel);
            Assert.AreEqual(tbDto, tbDto2, "tbDto and tbDto2 must be equal.");
        }

        [TestMethod]
        public void TestFindTransportbeziehungFail()
        {
            TransportbeziehungDTO tbDto = transportnetzServices.FindTransportbeziehung(
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Assert.IsNull(tbDto, "tbDto must be null.");
        }

        [TestMethod]
        public void TestFindTransportbeziehungenSuccess()
        {
            var lok1 = "Test-" + Guid.NewGuid();
            var lok2 = "Test-" + Guid.NewGuid();
            var lok3 = "Test-" + Guid.NewGuid();
            LokationDTO lokDto1 = new LokationDTO(lok1, TimeSpan.FromHours(12), 100m);
            LokationDTO lokDto2 = new LokationDTO(lok2, TimeSpan.FromDays(2), 80m);
            LokationDTO lokDto3 = new LokationDTO(lok3, TimeSpan.FromDays(2), 120m);

            transportnetzServices.CreateLokation(ref lokDto1);
            transportnetzServices.CreateLokation(ref lokDto2);
            transportnetzServices.CreateLokation(ref lokDto3);

            TransportbeziehungDTO tbDto1 = new TransportbeziehungDTO(lokDto1, lokDto2);
            TransportbeziehungDTO tbDto2 = new TransportbeziehungDTO(lokDto1, lokDto3);
            transportnetzServices.CreateTransportbeziehung(ref tbDto1);
            transportnetzServices.CreateTransportbeziehung(ref tbDto2);

            var tbs = transportnetzServices.FindTransportbeziehungen(lok1);
            Assert.IsTrue(tbs.Count() == 2);
        }
    }
}
