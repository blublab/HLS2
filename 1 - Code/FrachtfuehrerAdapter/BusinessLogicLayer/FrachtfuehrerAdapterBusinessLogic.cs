using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Util.Common.DataTypes;
using Util.Common.Interfaces;
using Util.MessagingServices.Implementations;
using Util.MessagingServices.Interfaces;

namespace ApplicationCore.FrachtfuehrerAdapter.BusinessLogicLayer
{
    internal class FrachtauftragDetailDTO : DTOType<FrachtauftragDetailDTO>
    {
        public int FaNr { get; set; }
        public int FrfNr { get; set; }
        public int FrvNr { get; set; }
        public DateTime PlanStartzeit { get; set; }
        public DateTime PlanEndezeit { get; set; }
        public int VerwendeteKapazitaetTEU { get; set; }
        public int VerwendeteKapazitaetFEU { get; set; }
    }

    internal class FrachtabrechnungDetailDTO : DTOType<FrachtabrechnungDetailDTO>
    {
        public virtual bool IstBestaetigt { get; set; }
        public virtual WaehrungsType Rechnungsbetrag { get; set; }
        public virtual int FaufNr { get; set; }

        public override string ToString()
        {
            return "Frachtabrechnung: " + " Bestätigt: " + this.IstBestaetigt + " FaufNr: " + this.FaufNr + " Betrag: " + this.Rechnungsbetrag;
        }
    }

    internal class FrachtfuehrerAdapterBusinessLogic
    {
        private IBuchhaltungServicesFuerFrachtfuehrerAdapter buchhaltungServices = null;

        public FrachtfuehrerAdapterBusinessLogic(IBuchhaltungServicesFuerFrachtfuehrerAdapter buchhaltungServices)
        {
            this.buchhaltungServices = buchhaltungServices;
        }

        public void SendeFrachtauftragAnFrachtfuehrer(FrachtauftragDTO fraDTO)
        {
            Contract.Requires(fraDTO != null);

            IMessagingServices messagingManager = null;
            IQueueServices<FrachtauftragDetailDTO> frachtauftragDetailQueue = null;

            System.Configuration.ConnectionStringSettings connectionSettings = System.Configuration.ConfigurationManager.ConnectionStrings["FrachtfuehrerExternalFrachtauftrag"];
            Contract.Assert(connectionSettings != null, "A FrachtfuehrerExternal connection setting needs to be defined in the App.config.");
            string frachtfuehrerQueue = connectionSettings.ConnectionString;
            Contract.Assert(string.IsNullOrEmpty(frachtfuehrerQueue) == false);

            messagingManager = MessagingServicesFactory.CreateMessagingServices();
            frachtauftragDetailQueue = messagingManager.CreateQueue<FrachtauftragDetailDTO>(frachtfuehrerQueue);
            FrachtauftragDetailDTO frachtauftragDetailSent = new FrachtauftragDetailDTO()
            { 
                FaNr = fraDTO.FraNr, 
                FrfNr = fraDTO.FrachtfuehrerRahmenvertrag.Frachtfuehrer.FrfNr, 
                FrvNr = fraDTO.FrachtfuehrerRahmenvertrag.FrvNr,
                PlanStartzeit = fraDTO.PlanStartzeit, 
                PlanEndezeit = fraDTO.PlanEndezeit, 
                VerwendeteKapazitaetTEU = fraDTO.VerwendeteKapazitaetTEU, 
                VerwendeteKapazitaetFEU = fraDTO.VerwendeteKapazitaetFEU 
            };
            frachtauftragDetailQueue.Send(frachtauftragDetailSent);
        }

        private void EmpfangVonFrachtabrechnungen()
        {
            System.Configuration.ConnectionStringSettings connectionSettings = System.Configuration.ConfigurationManager.ConnectionStrings["FrachtfuehrerExternalFrachtabrechnung"];
            Contract.Assert(connectionSettings != null, "A FrachtfuehrerExternal connection setting needs to be defined in the App.config.");
            string frachtfuehrerAuftragQueue = connectionSettings.ConnectionString;
            Contract.Assert(string.IsNullOrEmpty(frachtfuehrerAuftragQueue) == false);

            IMessagingServices ms = MessagingServicesFactory.CreateMessagingServices();
            IQueueServices<FrachtabrechnungDetailDTO> frachtabrechnungDetailDTOQueue = ms.CreateQueue<FrachtabrechnungDetailDTO>(frachtfuehrerAuftragQueue);

            while (true)
            {
                Console.WriteLine("Warte auf Frachtabrechnungen in Queue '" + frachtabrechnungDetailDTOQueue.Queue + "'.");

                FrachtabrechnungDetailDTO frachtauftragDetailReceived = frachtabrechnungDetailDTOQueue.ReceiveSync((o) =>
                {
                    return MessageAckBehavior.AcknowledgeMessage;
                });

                Console.WriteLine("Folgende Frachtabrechnung wurde aus der Queue " + frachtabrechnungDetailDTOQueue + " empfangen: " + frachtauftragDetailReceived.ToString());

                FrachtabrechnungDTO faDTO = new FrachtabrechnungDTO()
                {
                    IstBestaetigt = frachtauftragDetailReceived.IstBestaetigt,
                    Rechnungsbetrag = frachtauftragDetailReceived.Rechnungsbetrag,
                    FaufNr = frachtauftragDetailReceived.FaufNr
                };

                this.buchhaltungServices.PayFrachtabrechnung(ref faDTO);
            }
        }

        internal void StarteEmpfangVonFrachtabrechnungen()
        {
            var abrechnungsEmpfaenger = Task.Factory.StartNew(() => EmpfangVonFrachtabrechnungen());
            Task.WaitAll(abrechnungsEmpfaenger);
        }
    }
}
