using ApplicationCore.BankAdapter.BusinessLogicLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.DataTypes;
using Util.Common.Interfaces;
using Util.MessagingServices.Implementations;
using Util.MessagingServices.Interfaces;

namespace ApplicationCore.BankAdapter.BusinessLogicLayer
{
    internal class GutschriftDetailDTO : DTOType<GutschriftDetailDTO>
    {
        public int GutschriftNr { get; set; }
        public KontodatenType Kontodaten { get; set; }
        public WaehrungsType Betrag { get; set; }
    }

    internal class BankAdapterBusinessLogic
    {
        private string gutschriftQueueID = null;

        internal BankAdapterBusinessLogic()
        {
            System.Configuration.ConnectionStringSettings connectionSettings = System.Configuration.ConfigurationManager.ConnectionStrings["BankExternal"];
            Contract.Assert(connectionSettings != null, "A BankExternal connection setting needs to be defined in the App.config.");
            this.gutschriftQueueID = connectionSettings.ConnectionString;
            Contract.Assert(string.IsNullOrEmpty(gutschriftQueueID) == false);
        }

        internal void SendeGutschriftAnBank(GutschriftDTO gDTO)
        {
            IMessagingServices messagingManager = null;
            IQueueServices<GutschriftDetailDTO> gutschriftDetailQueue = null;

            messagingManager = MessagingServicesFactory.CreateMessagingServices();
            gutschriftDetailQueue = messagingManager.CreateQueue<GutschriftDetailDTO>(gutschriftQueueID);

            GutschriftDetailDTO gdDTO = new GutschriftDetailDTO()
            {
                GutschriftNr = gDTO.GutSchrNr,
                Kontodaten = gDTO.Kontodaten,
                Betrag = gDTO.Betrag
            };
            gutschriftDetailQueue.Send(gdDTO);
            Console.WriteLine("==> Gutschrift wurde an Bank gesendet");
        }
    }
}
