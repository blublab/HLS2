using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public class Program
    {
        public static void Main(string[] args)
        {
            IMessagingServices messagingManager = null;
            IQueueServices<FrachtauftragDetail> frachtauftragDetailQueue = null;

            messagingManager = MessagingServicesFactory.CreateMessagingServices();
            frachtauftragDetailQueue = messagingManager.CreateQueue<FrachtauftragDetail>("HLS.Queue.Frachtauftrag");
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
    }
}
