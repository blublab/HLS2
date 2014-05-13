using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Util.Common.DataTypes;
using Util.Common.Interfaces;
using Util.MessagingServices.Implementations;
using Util.MessagingServices.Interfaces;

namespace BankExternal
{
    internal class GutschriftDetail : DTOType<GutschriftDetail>
    {
        public int GutschriftNr { get; set; }
        public KontodatenType Kontodaten { get; set; }
        public WaehrungsType Betrag { get; set; }

        public override string ToString()
        {
            return "Gutschrift: Nr." + this.GutschriftNr + " Kto-Daten: " + this.Kontodaten + " Betrag: " + this.Betrag;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var gutschrift = Task.Factory.StartNew(() => ReceiveGutschriften());

            Task.WaitAll(gutschrift); //Lässt Console geöffnet
        }

        private static void ReceiveGutschriften()
        {
            System.Configuration.ConnectionStringSettings connectionSettings = System.Configuration.ConfigurationManager.ConnectionStrings["BankExternal"];
            Contract.Assert(connectionSettings != null, "A BankExternal connection setting needs to be defined in the App.config.");
            string gutschriftQueueID = connectionSettings.ConnectionString;
            Contract.Assert(string.IsNullOrEmpty(gutschriftQueueID) == false);

            IMessagingServices messagingManager = null;
            IQueueServices<GutschriftDetail> gutschriftDetailQueue = null;

            messagingManager = MessagingServicesFactory.CreateMessagingServices();
            gutschriftDetailQueue = messagingManager.CreateQueue<GutschriftDetail>(gutschriftQueueID);

            Console.WriteLine("Warte auf Gutschriften in Queue '" + gutschriftDetailQueue.Queue + "'.");

            while (true)
            {
                GutschriftDetail gutschriftEmpfangen = gutschriftDetailQueue.ReceiveSync((o) =>
                {
                    return MessageAckBehavior.AcknowledgeMessage;
                });
                Console.Beep();
                Console.WriteLine("<== Gutschrift empfangen.");
            }
        }
    }
}

