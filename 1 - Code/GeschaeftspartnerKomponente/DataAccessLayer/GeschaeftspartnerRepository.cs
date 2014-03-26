using System.Runtime.CompilerServices;
using Util.PersistenceServices.Interfaces;

namespace ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer
{
    internal class GeschaeftspartnerRepository
    {
        private readonly IPersistenceServices persistenceService;

        public GeschaeftspartnerRepository(IPersistenceServices persistenceService)
        {
            this.persistenceService = persistenceService;
        }

        public void SaveGeschaeftspartner(Geschaeftspartner gp)
        {
            persistenceService.Save(gp);
        }

        public void SaveAdresse(Adresse ad)
        {
            persistenceService.Save(ad);
        }

        public Geschaeftspartner FindByGpNr(int gpNr)
        {
            return persistenceService.GetById<Geschaeftspartner, int>(gpNr);
        }

        public Adresse FindByAdId(int adId)
        {
            return persistenceService.GetById<Adresse, int>(adId);
        }
    }
}
