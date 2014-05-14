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

namespace FrachtfuehrerExternal
{
    internal class FrachtauftragDetail : DTOType<FrachtauftragDetail>
    {
        public int FaNr { get; set; }
        public int FrfNr { get; set; }
        public int FrvNr { get; set; }
        public DateTime PlanStartzeit { get; set; }
        public DateTime PlanEndezeit { get; set; }
        public int VerwendeteKapazitaetTEU { get; set; }
        public int VerwendeteKapazitaetFEU { get; set; }

        public override string ToString()
        {
            return "Frachtauftrag: " + this.FaNr + " Frf: " + this.FrfNr + " Frv: " + this.FrvNr + " Start: " + this.PlanStartzeit + " Ende: " + this.PlanEndezeit + " TEU: " + this.VerwendeteKapazitaetTEU + " FEU: " + this.VerwendeteKapazitaetFEU;
        }
    }

    internal class FrachtabrechnungDetail : DTOType<FrachtabrechnungDetail>
    {
        public virtual bool IstBestaetigt { get; set; }
        public virtual WaehrungsType Rechnungsbetrag { get; set; }
        public virtual int FaufNr { get; set; }

        public override string ToString()
        {
            return "Frachtabrechnung: " + " Bestätigt: " + this.IstBestaetigt + " FaufNr: " + this.FaufNr + " Betrag: " + this.Rechnungsbetrag;
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            System.Configuration.ConnectionStringSettings auftragConnectionSettings = System.Configuration.ConfigurationManager.ConnectionStrings["FrachtfuehrerExternalFrachtauftrag"];
            Contract.Assert(auftragConnectionSettings != null, "A FrachtfuehrerExternal connection setting needs to be defined in the App.config.");
            string frachtfuehrerAuftragQueueID = auftragConnectionSettings.ConnectionString;
            Contract.Assert(string.IsNullOrEmpty(frachtfuehrerAuftragQueueID) == false);

            System.Configuration.ConnectionStringSettings abrechnungConnectionSettings = System.Configuration.ConfigurationManager.ConnectionStrings["FrachtfuehrerExternalFrachtabrechnung"];
            Contract.Assert(abrechnungConnectionSettings != null, "A FrachtfuehrerExternal connection setting needs to be defined in the App.config.");
            string frachtfuehrerAbrechnungQueueID = abrechnungConnectionSettings.ConnectionString;
            Contract.Assert(string.IsNullOrEmpty(frachtfuehrerAbrechnungQueueID) == false);

            var receiver = Task.Factory.StartNew(() => Receiver(frachtfuehrerAuftragQueueID));
            var sender = Task.Factory.StartNew(() => Sender(frachtfuehrerAbrechnungQueueID));

            Task.WaitAll(receiver, sender);
        }

        private static void Receiver(string frachtfuehrerAuftragQueue)
        {
            IMessagingServices ms = MessagingServicesFactory.CreateMessagingServices();
            IQueueServices<FrachtauftragDetail> frachtauftragDetailQueue = ms.CreateQueue<FrachtauftragDetail>(frachtfuehrerAuftragQueue);
            Console.WriteLine("Warte auf Frachtaufträge in Queue '" + frachtauftragDetailQueue.Queue + "'.");

            while (true)
            {
                FrachtauftragDetail frachtauftragDetailReceived = frachtauftragDetailQueue.ReceiveSync((o) =>
                {
                    return MessageAckBehavior.AcknowledgeMessage;
                });
                Console.WriteLine("Frachtauftrag empfangen: " + frachtauftragDetailReceived.ToString());
            }
        }

        private static void Sender(string frachtfuehrerAbrechnungQueueID)
        {
            IMessagingServices ms = MessagingServicesFactory.CreateMessagingServices();
            IQueueServices<FrachtabrechnungDetail> frachtabrechnungDetailQueue = ms.CreateQueue<FrachtabrechnungDetail>(frachtfuehrerAbrechnungQueueID);

            for (int i = 1; i <= 3; i++)
            {
                Thread.Sleep(500);
                FrachtabrechnungDetail dummyFrachtabrechnungDetail = new FrachtabrechnungDetail() { IstBestaetigt = true, Rechnungsbetrag = new WaehrungsType(i), FaufNr = i };
                frachtabrechnungDetailQueue.Send(dummyFrachtabrechnungDetail);
                Console.WriteLine("Folgende Frachtabrechnung wurde in die Queue " + frachtfuehrerAbrechnungQueueID + " geschoben: " + dummyFrachtabrechnungDetail.ToString());
                Console.Beep();
            }
        }
    }
}
