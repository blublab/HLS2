using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using BuchhaltungKomponente.BusinessLogicLayer;
using Common.DataTypes;
using Common.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.PersistenceServices.Interfaces;

namespace ApplicationCore.BuchhaltungKomponente.AccessLayer
{
    public class BuchhaltungKomponenteFacade : IBuchhaltungServices, IBuchhaltungServicesFuerFrachtfuehrerAdapter
    {
        private readonly BuchhaltungRepository bh_REPO;
        private readonly ITransactionServices transactionService;
        private readonly BuchhaltungKomponenteBusinessLogic bh_BL;
        private readonly IBankAdapterServicesFuerBuchhaltung bankServices;

        public BuchhaltungKomponenteFacade(
                IPersistenceServices persistenceService,
                ITransactionServices transactionService,
                IBankAdapterServicesFuerBuchhaltung bankServices)
        {
            Check.Argument(persistenceService != null, "persistenceService != null");
            Check.Argument(transactionService != null, "transactionService != null");
            Check.Argument(bankServices != null, "bankServices != null");

            this.bh_REPO = new BuchhaltungRepository(persistenceService);
            this.transactionService = transactionService;
            this.bankServices = bankServices;
            this.bh_BL = new BuchhaltungKomponenteBusinessLogic();
        }

        public void SetzeUnterbeauftragungServices(IUnterbeauftragungServicesFuerBuchhaltung unterbeauftragungServices)
        {
 // FIXME: Methode existiert (noch?) nicht.
 //           this.bh_BL.setUnterbeauftragungServices(unterbeauftragungServices);
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
        #endregion

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
    }
}
