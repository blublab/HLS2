using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using Common.Implementations;
using System;
using Util.PersistenceServices.Interfaces;

namespace ApplicationCore.GeschaeftspartnerKomponente.AccessLayer
{
    public class GeschaeftspartnerKomponenteFacade : IGeschaeftspartnerServices
    {
        private readonly GeschaeftspartnerRepository gp_REPO;
        private readonly ITransactionServices transactionService;

        public GeschaeftspartnerKomponenteFacade(IPersistenceServices persistenceService, ITransactionServices transactionService)
        {
            Check.Argument(persistenceService != null, "persistenceService != null");
            Check.Argument(transactionService != null, "transactionService != null");

            this.gp_REPO = new GeschaeftspartnerRepository(persistenceService);
            this.transactionService = transactionService;
        }

        public void CreateGeschaeftspartner(ref GeschaeftspartnerDTO gpDTO)
        {
            Check.Argument(gpDTO != null, "gpDTO != null");
            Check.Argument(gpDTO.GpNr == 0, "gpDTO.Id == 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Geschaeftspartner gp = gpDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () => 
                {
                    this.gp_REPO.Save(gp);
                });
            gpDTO = this.FindGeschaeftspartner(gp.GpNr);
        }

        public void UpdateGeschaeftspartner(ref GeschaeftspartnerDTO gpDTO)
        {
            Check.Argument(gpDTO != null, "gpDTO != null");
            Check.Argument(gpDTO.GpNr > 0, "gpDTO.Id > 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");
            int gpNr = gpDTO.GpNr;
            transactionService.ExecuteTransactional(() =>
            {
                if (this.gp_REPO.FindByGpNr(gpNr) == null)
                {
                    throw new GeschaeftspartnerNichtGefundenException(gpNr);
                }
            });

            Geschaeftspartner gp = gpDTO.ToEntity();
            transactionService.ExecuteTransactional(() =>
                {
                    this.gp_REPO.Save(gp);
                });
            gpDTO = this.FindGeschaeftspartner(gp.GpNr);
        }

        public GeschaeftspartnerDTO FindGeschaeftspartner(int gpNr)
        {
            Check.Argument(gpNr > 0, "gpNr > 0");

            Geschaeftspartner gp = null;
            transactionService.ExecuteTransactionalIfNoTransactionProvided(
                () =>
                {
                    gp = this.gp_REPO.FindByGpNr(gpNr);
                });

            if (gp == null)
            {
                return null;
            }

            return gp.ToDTO();
        }
    }
}
