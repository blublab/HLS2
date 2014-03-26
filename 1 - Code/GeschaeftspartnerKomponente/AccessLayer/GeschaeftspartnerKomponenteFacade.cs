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
                    this.gp_REPO.SaveGeschaeftspartner(gp);
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
                    this.gp_REPO.SaveGeschaeftspartner(gp);
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

        public AdresseDTO FindAdresse(int adId)
        {
            Check.Argument(adId > 0, "adId > 0");
            
            Adresse ad = null;
            transactionService.ExecuteTransactional(
                () =>
                {
                    ad = this.gp_REPO.FindByAdId(adId);
                });
            if (ad == null)
            {
                return null;
            }

            return ad.ToDTO();
        }

        public void CreateAdresse(ref AdresseDTO adDTO)
        {
            Check.Argument(adDTO != null, "adDTO != null");
            Check.Argument(adDTO.Id == 0, "adDTO.Id == 0");
            Check.OperationCondition(!transactionService.IsTransactionActive, "Keine aktive Transaktion erlaubt.");

            Adresse ad = adDTO.ToEntity();
            transactionService.ExecuteTransactional(
                () =>
                {
                    this.gp_REPO.SaveAdresse(ad);
                });
            adDTO = this.FindAdresse(ad.Id);
        }
    }
}
