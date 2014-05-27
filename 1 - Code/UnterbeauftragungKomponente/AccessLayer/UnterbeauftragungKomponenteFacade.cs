using ApplicationCore.AuftragKomponente.AccessLayer;
using ApplicationCore.AuftragKomponente.DataAccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.BusinessLogicLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using Common.Implementations;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using Util.MailServices.Interfaces;
using Util.PersistenceServices.Interfaces;

namespace ApplicationCore.UnterbeauftragungKomponente.AccessLayer
{
    public class UnterbeauftragungKomponenteFacade : IUnterbeauftragungServices, IUnterbeauftragungServicesFürTransportplanung, IUnterbeauftragungServicesFuerBuchhaltung, IUnterbeauftragungServicesFuerAuftrag
    {
        private readonly ITransactionServices transactionService;
        private readonly FrachtfuehrerRahmenvertragRepository frv_REPO;
        private readonly FrachtfuehrerRepository frf_REPO;
        private readonly UnterbeauftragungKomponenteBusinessLogic ubK_BL;
        private readonly FrachtauftragRepository fra_REPO;
        private readonly FrachtbriefRepository frb_REPO;
        private readonly IGeschaeftspartnerServices geschaeftspartnerService;
        private readonly IPDFErzeugungsServicesFuerUnterbeauftragung pdfService;
        private readonly IMailServices mailService;

        public UnterbeauftragungKomponenteFacade(
            IPersistenceServices persistenceService, 
            ITransactionServices transactionService, 
            IFrachtfuehrerServicesFürUnterbeauftragung frachtfuehrerServices, 
            IGeschaeftspartnerServices geschaeftspartnerServices,
            IPDFErzeugungsServicesFuerUnterbeauftragung pdfServices,
            IMailServices mailServices)
        {
            Check.Argument(persistenceService != null, "persistenceService != null");
            Check.Argument(transactionService != null, "transactionService != null");
            this.transactionService = transactionService;
            this.frv_REPO = new FrachtfuehrerRahmenvertragRepository(persistenceService);
            this.frf_REPO = new FrachtfuehrerRepository(persistenceService);
            this.ubK_BL = new UnterbeauftragungKomponenteBusinessLogic(persistenceService, frachtfuehrerServices);
            this.fra_REPO = new FrachtauftragRepository(persistenceService);
            this.frb_REPO = new FrachtbriefRepository(persistenceService);
            this.geschaeftspartnerService = geschaeftspartnerServices;
            this.pdfService = pdfServices;
            this.mailService = mailServices;
        }

        public void CreateFrachtfuehrerRahmenvertrag(ref FrachtfuehrerRahmenvertragDTO frvDTO)
        {
            Check.Argument(frvDTO != null, "frvDTO != null");
            Check.Argument(frvDTO.FrvNr == 0, "frvDTO.FrfNr == 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");
            
            FrachtfuehrerRahmenvertrag frv = frvDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () =>
                {
                    this.frv_REPO.Save(frv);
                });
            frvDTO = frv.ToDTO();
        }

        public void CreateFrachtfuehrer(ref FrachtfuehrerDTO frfDTO)
        {
            Check.Argument(frfDTO != null, "frfDTO != null");
            Check.Argument(frfDTO.FrfNr == 0, "frfDTO.FrfNr == 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Frachtfuehrer frf = frfDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () =>
                {
                    this.frf_REPO.Save(frf);
                });
            frfDTO = frf.ToDTO();
        }

        public List<FrachtfuehrerRahmenvertrag> FindGültigFür(long tbNr, DateTime zeitspanneVon, DateTime zeitspanneBis)
        {
            Check.Argument(tbNr >= 0, "tbNr >= 0");
            Check.Argument(zeitspanneVon <= zeitspanneBis, "zeitspanneVon <= zeitspanneBis");

            return transactionService.ExecuteTransactionalIfNoTransactionProvided(
                () => 
                {
                    return this.frv_REPO.FindGültigFür(tbNr, zeitspanneVon, zeitspanneBis);
                });
        }

        public int BeauftrageTransport(int frvNr, DateTime planStartzeit, DateTime planEndezeit, int verwendeteKapazitaetTEU, int verwendeteKapazitaetFEU)
        {
            Check.Argument(frvNr > 0, "frvNr > 0");
            Check.Argument(planStartzeit <= planEndezeit, "planStartzeit <= planEndezeit");
            Check.Argument(verwendeteKapazitaetTEU >= 0, "verwendeteKapazitaetTEU >= 0");
            Check.Argument(verwendeteKapazitaetFEU >= 0, "verwendeteKapazitaetFEU >= 0");
            Check.Argument(verwendeteKapazitaetTEU + verwendeteKapazitaetFEU >= 1, "verwendeteKapazitaetTEU+verwendeteKapazitaetFEU >= 1");

            return transactionService.ExecuteTransactionalIfNoTransactionProvided(
                () => 
                {
                    FrachtfuehrerRahmenvertrag frv = this.frv_REPO.FindByFrvNr(frvNr);
                    if (frv == null)
                    {
                        throw new FrachtfuehrerRahmenvertragNichtGefundenException(frvNr);
                    }
                    return this.ubK_BL.BeaufrageTransport(frv, planStartzeit, planEndezeit, verwendeteKapazitaetTEU, verwendeteKapazitaetFEU); 
                });
        }

        #region IUnterbeauftragungServicesFuerBuchhaltung
        public void SchliesseFrachtauftragAb(int faufNr)
        {
            Check.Argument(faufNr >= 0, "faufNr >= 0");

            transactionService.ExecuteTransactional(
                () =>
                {
                    Frachtauftrag fauf = this.fra_REPO.FindByFaufNr(faufNr);
                    fauf.Status = FrachtauftragStatusTyp.Abgeschlossen;
                    this.fra_REPO.Add(fauf);
                });
        }

        public bool PruefeObFrachtauftragVorhanden(int faufNr)
        {
            if (faufNr <= 0)
            {
                return false;
            }
            try
            {          
                transactionService.ExecuteTransactional(
                () =>
                {
                    Frachtauftrag fauf = this.fra_REPO.FindByFaufNr(faufNr);
                    fauf.Status = FrachtauftragStatusTyp.Abgeschlossen;
                    this.fra_REPO.Add(fauf);
                });
            }
            catch
            { //TODO check which exception is thrown
                return false;
            }
            return true;
        }
        #endregion

        public void CreateFrachtauftrag(ref FrachtauftragDTO faufDTO)
        {
            Check.Argument(faufDTO != null, "frfDTO != null");
            Check.Argument(faufDTO.FraNr == 0, "frfDTO.FrfNr == 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Frachtauftrag fauf = faufDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () =>
                {
                    this.fra_REPO.Add(fauf);
                });
            faufDTO = fauf.ToDTO();
        }

        public void CreateFrachtbrief(ref FrachtbriefDTO frbDTO)
        {
            Check.Argument(frbDTO != null, "frbDTO != null");
            Check.Argument(frbDTO.FrbNr == 0, "frbDTO.FrbNr == 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Frachtbrief frb = frbDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () =>
                {
                    this.frb_REPO.Add(frb);
                });
            frbDTO = frb.ToDTO();
        }

        public FrachtauftragDTO ReadFrachtauftragByID(int faufNr)
        {
            Check.Argument(faufNr > 0, "faufNr > 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Frachtauftrag fauf = null;
            transactionService.ExecuteTransactional(
                 () =>
                 {
                     fauf = this.fra_REPO.FindByFaufNr(faufNr);
                 });
            return fauf.ToDTO();
        }

        #region IUnterbeauftragungServicesFuerAuftrag
        public void ErstelleFrachtbriefUndVerschickeIhn(SendungsanfrageDTO saDTO)
        {
            Frachtbrief frbr = new Frachtbrief();
            frbr.AbsenderAnschrift = new Adresse() { Land = "Deutschland", Wohnort = "Hamburg", PLZ = "20099", Strasse = "Berliner Tor", Hausnummer = "7" };
            frbr.AbsenderName = "HAW Logistic System";
            frbr.AusstellungsZeit = DateTime.Now;
            GeschaeftspartnerDTO gpDTO = geschaeftspartnerService.FindGeschaeftspartner(saDTO.AuftrageberNr);
            frbr.EmpfaengerName = gpDTO.Vorname + " " + gpDTO.Nachname;
            frbr.EmpfaengerAnschrift = gpDTO.Adressen[0].ToEntity();
            long start = saDTO.StartLokation;
            long ziel = saDTO.ZielLokation;
            transactionService.ExecuteTransactional(
                 () =>
                 {
                    this.frb_REPO.Add(frbr);
                 });
            string pdfPath = pdfService.ErzeugeFrachtbriefPDF(frbr.ToDTO());
            if (pdfPath != null)
            {
                MailMessage msg = new MailMessage();
                Attachment atchmnt = new Attachment(pdfPath);
                msg.Attachments.Add(atchmnt);
                msg.Body = "Guten Tag,\n anbei der Frachtbrief.\n\n Viele Grüsse,\n Ihr HLS-TEAM";
                msg.Subject = "HLS: Frachtbrief";
                mailService.SendMail(msg);
            }
        }
        #endregion
    }
}
