using ApplicationCore.AuftragKomponente.AccessLayer;
using ApplicationCore.AuftragKomponente.DataAccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.AccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using ApplicationCore.TransportnetzKomponente.AccessLayer;
using ApplicationCore.TransportnetzKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.AccessLayer;
using ApplicationCore.TransportplanungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using BuchhaltungKomponente.BusinessLogicLayer;
using Common.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Util.Common.DataTypes;
using Util.MailServices.Interfaces;
using Util.PersistenceServices.Interfaces;

namespace ApplicationCore.BuchhaltungKomponente.AccessLayer
{
    public class BuchhaltungKomponenteFacade : IBuchhaltungServices, IBuchhaltungServicesFuerFrachtfuehrerAdapter, IBuchhaltungsServicesFuerBank
    {
        private readonly BuchhaltungRepository bh_REPO;
        private readonly ITransactionServices transactionService;
        private readonly BuchhaltungKomponenteBusinessLogic bh_BL;
        private readonly IBankAdapterServicesFuerBuchhaltung bankServices;
        private readonly ITransportplanServicesFuerBuchhaltung transportplanServiceFuerBuchhaltung;
        private readonly IAuftragServicesFuerBuchhaltung auftragServiceFuerBuchhaltung;
        private readonly IGeschaeftspartnerServices geschaeftspartnerService;
        private readonly IPDFErzeugungsServicesFuerBuchhaltung pdfErzeugungsServiceFuerBuchhaltung;
        private readonly IMailServices mailService;
        private readonly ITransportnetzServices tpNetzService;

        public BuchhaltungKomponenteFacade(
                IPersistenceServices persistenceService,
                ITransactionServices transactionService,
                IBankAdapterServicesFuerBuchhaltung bankServices,
                ITransportplanServicesFuerBuchhaltung transportplanServicesFuerBuchhaltung,
                IAuftragServicesFuerBuchhaltung auftragServicesFuerBuchhaltung,
                IGeschaeftspartnerServices geschaeftspartnerServices,
                IPDFErzeugungsServicesFuerBuchhaltung pdfErzeugungsServicesFuerBuchhaltung,
                IMailServices mailServices,
                ITransportnetzServices tpnetzServices)
        {
            Check.Argument(persistenceService != null, "persistenceService != null");
            Check.Argument(transactionService != null, "transactionService != null");
            Check.Argument(bankServices != null, "bankServices != null");
            Check.Argument(transportplanServicesFuerBuchhaltung != null, "transportplanServicesFuerBuchhaltung != null");
            Check.Argument(auftragServicesFuerBuchhaltung != null, "auftragServicesFuerBuchhaltung != null");
            Check.Argument(geschaeftspartnerServices != null, "geschaeftspartnerServices != null");
            Check.Argument(pdfErzeugungsServicesFuerBuchhaltung != null, "pdfErzeugungsServicesFuerBuchhaltung != null");
            Check.Argument(mailServices != null, "mailServices != null");

            this.bh_REPO = new BuchhaltungRepository(persistenceService);
            this.transactionService = transactionService;
            this.bankServices = bankServices;
            this.bh_BL = new BuchhaltungKomponenteBusinessLogic();
            this.transportplanServiceFuerBuchhaltung = transportplanServicesFuerBuchhaltung;
            this.auftragServiceFuerBuchhaltung = auftragServicesFuerBuchhaltung;
            this.geschaeftspartnerService = geschaeftspartnerServices;
            this.pdfErzeugungsServiceFuerBuchhaltung = pdfErzeugungsServicesFuerBuchhaltung;
            this.mailService = mailServices;
            this.tpNetzService = tpnetzServices;
        }

        public void SetzeUnterbeauftragungServices(IUnterbeauftragungServicesFuerBuchhaltung unterbeauftragungServices)
        {
            this.bh_BL.SetUnterbeauftragungServices(unterbeauftragungServices);
        }

        #region IBuchhaltungServices
        public FrachtabrechnungDTO CreateFrachtabrechnung(int faufNr)
        {
            Check.Argument(faufNr > 0, "faufNr > 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            if (bh_BL.PruefeObFrachtauftragVorhanden(faufNr) == false)
            {
                throw new FrachtauftragNichtGefundenException(faufNr);
            }
            Frachtabrechnung fab = new Frachtabrechnung();
            fab.FaufNr = faufNr;
            transactionService.ExecuteTransactional(
                 () =>
                 {
                     this.bh_REPO.SaveFrachtabrechnung(fab);
                 });
            return fab.ToDTO();
        }

        public void DeleteFrachtabrechnung(ref FrachtabrechnungDTO fabDTO)
        {
            Frachtabrechnung fab = fabDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () =>
                {
                    this.bh_REPO.DeleteFrachtabrechnung(fab);
                });
        }

        public void CreateKundenrechnung(ref KundenrechnungDTO krDTO)
        {
            Check.Argument(krDTO != null, "krDTO != null");
            Check.Argument(krDTO.RechnungsNr == -1, "krDTO.RechnungsNr == -1");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Kundenrechnung kr = krDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () =>
                {
                    this.bh_REPO.SpeichereKundenrechnung(kr);
                });
            krDTO = this.FindKundenrechnung(kr.RechnungsNr);
        }

        public KundenrechnungDTO FindKundenrechnung(int krNr)
        {
            Check.Argument(krNr > 0, "krNr > 0");

            Kundenrechnung kr = null;
            transactionService.ExecuteTransactionalIfNoTransactionProvided(
                () =>
                {
                    kr = this.bh_REPO.GetKundenrechnungById(krNr);
                });

            if (kr == null)
            {
                return null;
            }

            return kr.ToDTO();
        }

        public KundenrechnungDTO ErstelleKundenrechnung(int tpNr, int saNr)
        {
            Check.Argument(tpNr > 0, "TpNr > 0");
            Check.Argument(saNr > 0, "saNr > 0");

            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Kundenrechnung kr = new Kundenrechnung();
            kr.RechnungBezahlt = false;
            TransportplanDTO tpDTO = transportplanServiceFuerBuchhaltung.FindeTransportplanUeberTpNr(tpNr);
            IList<TransportplanSchrittDTO> tpSchritte = tpDTO.TransportplanSchritte;

            decimal kostenSumme = 0;
            foreach (TransportplanSchrittDTO tpsDTO in tpSchritte)
            {
                kostenSumme += tpsDTO.Kosten;
            }

            List<string> wege = new List<string>();
            foreach (TransportAktivitaetDTO tpaDTO in tpSchritte)
            {
                long tpBeziehung = tpaDTO.FuerTransportAufTransportbeziehung;
                TransportbeziehungDTO tpbDTO = tpNetzService.FindTransportbeziehung(tpBeziehung);
                string start = tpbDTO.Start.ToString();
                string ziel = tpbDTO.Ziel.ToString();
                wege.Add(start + " - " + ziel);
            }

            kr.Rechnungsbetrag = new WaehrungsType(kostenSumme);
            kr.Sendungsanfrage = saNr;
            SendungsanfrageDTO saDTO = auftragServiceFuerBuchhaltung.FindeSendungsanfrageUeberSaNr(saNr);
            GeschaeftspartnerDTO gpDTO = geschaeftspartnerService.FindGeschaeftspartner(saDTO.AuftrageberNr);
            if (gpDTO.Adressen.Count > 0)
            {
                kr.Rechnungsadresse = gpDTO.Adressen.First<AdresseDTO>().Id;
            }
            transactionService.ExecuteTransactional(
                () =>
                {
                    bh_REPO.SpeichereKundenrechnung(kr);
                });
            KundenrechnungDTO krDTO = kr.ToDTO();
            string pdfPath = pdfErzeugungsServiceFuerBuchhaltung.ErstelleKundenrechnungPDF(krDTO, tpSchritte, gpDTO, wege);
            if (pdfPath != null)
            {
                MailMessage msg = new MailMessage();
                Attachment atchmnt = new Attachment(pdfPath);
                msg.Attachments.Add(atchmnt);
                msg.Body = "Guten Tag,\n anbei die Kundenrechnung.\n\n Viele Grüsse,\n Ihr HLS-TEAM";
                msg.Subject = "HLS: Kundenrechnung";
                mailService.SendMail(msg);
            }
            return kr.ToDTO();
        }

        public IList<KundenrechnungDTO> GetKundenrechnungen()
        {
            return bh_REPO.GetKundenrechnungen();
        }

        #endregion IBuchhaltungServices

        #region IBuchhaltungServicesFuerFrachtfuehrerAdapter
        public void PayFrachtabrechnung(ref FrachtabrechnungDTO fabDTO)
        {
            Check.Argument(fabDTO != null, "fabDTO != null");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Frachtabrechnung fab = fabDTO.ToEntity();
            int faufNr = fab.FaufNr;
            bh_BL.SchliesseFrachtauftragAb(faufNr);
            Gutschrift gutschrift = new Gutschrift();
            gutschrift.Betrag = fab.Rechnungsbetrag;
            fab.Gutschrift = gutschrift;
            GutschriftDTO gutschriftDTO = gutschrift.ToDTO();
            transactionService.ExecuteTransactional(
                () =>
                {
                    this.bh_REPO.SaveFrachtabrechnung(fab);
                });
            bankServices.SendeGutschriftAnBank(ref gutschriftDTO);
            fabDTO = fab.ToDTO();
        }

        public FrachtabrechnungDTO ReadFrachtabrechnungByID(int fabNr)
        {
            Check.Argument(fabNr > 0, "fabNr > 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Frachtabrechnung fab = null;
            transactionService.ExecuteTransactional(
                 () =>
                 {
                     fab = this.bh_REPO.GetFrachtabrechnungById(fabNr);
                 });
            return fab.ToDTO();
        }
        #endregion

        #region IBuchhaltungsServicesFuerBank
        public KundenrechnungDTO VerarbeiteZahlungseingang(ref ZahlungseingangDTO zeDTO)
        {
            Kundenrechnung kr = null;

            Zahlungseingang ze = zeDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () =>
                {
                    bh_REPO.SpeichereZahlungseingang(ze);
                });
            transactionService.ExecuteTransactional(
                () =>
                {
                    kr = bh_REPO.GetKundenrechnungById(ze.KrNr);
                });
            kr.Rechnungsbetrag -= ze.Zahlungsbetrag;
            if (kr.Rechnungsbetrag.Wert <= 0)
            {
                kr.RechnungBezahlt = true;
                auftragServiceFuerBuchhaltung.SchliesseSendungsanfrageAb(kr.Sendungsanfrage);
            }
            transactionService.ExecuteTransactional(
                () =>
                {
                    bh_REPO.SpeichereKundenrechnung(kr);
                });
            return kr.ToDTO();
        }
        #endregion
    }
}
