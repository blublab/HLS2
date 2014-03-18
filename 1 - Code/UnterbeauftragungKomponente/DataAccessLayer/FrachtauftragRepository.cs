using Util.PersistenceServices.Interfaces;

namespace ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer
{
    internal class FrachtauftragRepository
    {
        private readonly IPersistenceServices persistenceService;

        public FrachtauftragRepository(IPersistenceServices persistenceService)
        {
            this.persistenceService = persistenceService;
        }

        public void Add(Frachtauftrag fa)
        {
            persistenceService.Save(fa);
        }
    }
}
