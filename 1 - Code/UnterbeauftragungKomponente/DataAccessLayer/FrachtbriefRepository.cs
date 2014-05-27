using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using Util.PersistenceServices.Interfaces;

namespace ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer
{
    internal class FrachtbriefRepository
    {
        private readonly IPersistenceServices persistenceService;

        public FrachtbriefRepository(IPersistenceServices persistenceService)
        {
            this.persistenceService = persistenceService;
        }

        public void Add(Frachtbrief frb)
        {
            persistenceService.Save(frb);
        }

        internal Frachtbrief FindByFaufNr(int frbNr)
        {
            return persistenceService.GetById<Frachtbrief, int>(frbNr);
        }
    }
}
