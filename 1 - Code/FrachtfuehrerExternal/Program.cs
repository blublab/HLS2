using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Util.Common.DataTypes;
using Util.Common.Interfaces;
using Util.MessagingServices.Implementations;
using Util.MessagingServices.Interfaces;

namespace FrachtfuehrerExternal
{
    // Dummy
    internal class Gutschrift
    {
    }

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
        public virtual int FabNr { get; set; }
        public virtual bool IstBestaetigt { get; set; }
        public virtual WaehrungsType Rechnungsbetrag { get; set; }
        public virtual int FaufNr { get; set; }
        public virtual Gutschrift Gutschrift { get; set; }
        public virtual int RechnungsNr { get; set; }

        public override string ToString()
        {
            return "Frachtabrechnung: " + this.FabNr + " Bestätigt: " + this.IstBestaetigt + " FaufNr: " + this.FaufNr + " Betrag: " + this.Rechnungsbetrag + " Rechn. Nr.: " + this.RechnungsNr;
        }
    }

    public class FrachtauftragReceiver
    {
        private IQueueServices<FrachtauftragDetail> frachtauftragDetailQueue = null;

        public FrachtauftragReceiver(ref IMessagingServices ms)
        {
            this.frachtauftragDetailQueue = ms.CreateQueue<FrachtauftragDetail>("HLS.Queue.Frachtauftrag");
        }

        public void Run()
        {
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

    internal class FrachtabrechnungSender
    {
        private IQueueServices<FrachtabrechnungDetail> frachtabrechnungDetailQueue = null;

        public FrachtabrechnungSender(ref IMessagingServices ms)
        {
            frachtabrechnungDetailQueue = ms.CreateQueue<FrachtabrechnungDetail>("HLS.Queue.Frachtabrechnung.Team5");
        }

        public void Run()
        {
            // TODO Frachabrechnungen verschicken
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            IMessagingServices messagingManager = null;
            int result = 0;

            messagingManager = MessagingServicesFactory.CreateMessagingServices();

            FrachtauftragReceiver receiver = new FrachtauftragReceiver(ref messagingManager);
            FrachtabrechnungSender sender = new FrachtabrechnungSender(ref messagingManager);

            System.Threading.Thread receiverThread = new Thread(new ThreadStart(receiver.Run));
            System.Threading.Thread senderThread = new Thread(new ThreadStart(sender.Run));

            try
            {
                receiverThread.Start();
                senderThread.Start();

                receiverThread.Join();
                senderThread.Join();
            }
            catch (ThreadStateException e)
            {
                Console.WriteLine(e);
                result = 1;
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e);
                result = 1;
            }
            finally
            {
                //TODO dispose messagingServices?
                Environment.ExitCode = result;
            }
        }
    }
}
