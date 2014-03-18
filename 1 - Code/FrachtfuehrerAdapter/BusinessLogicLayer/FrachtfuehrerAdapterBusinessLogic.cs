using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
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

    internal class FrachtfuehrerAdapterBusinessLogic
    {
        public FrachtfuehrerAdapterBusinessLogic()
        {
        }

        public void SendeFrachtauftragAnFrachtfuehrer(FrachtauftragDTO fraDTO)
        {
            Contract.Requires(fraDTO != null);

            IMessagingServices messagingManager = null;
            IQueueServices<FrachtauftragDetailDTO> frachtauftragDetailQueue = null;

            System.Configuration.ConnectionStringSettings connectionSettings = System.Configuration.ConfigurationManager.ConnectionStrings["FrachtfuehrerExternal"];
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
    }
}
