using System.Collections.Generic;
using System.Linq;
using Util.PersistenceServices.Interfaces;

namespace ApplicationCore.BuchhaltungKomponente.DataAccessLayer
{
    internal class BuchhaltungRepository
    {
        private readonly IPersistenceServices persistenceService;

        public BuchhaltungRepository(IPersistenceServices persistenceService)
        {
            this.persistenceService = persistenceService;
        }

        public void SaveFrachtabrechnung(Frachtabrechnung fab)
        {
            persistenceService.Save(fab);
        }

        public void DeleteFrachtabrechnung(Frachtabrechnung fab)
        {
            persistenceService.Delete<Frachtabrechnung>(fab);
        }

        public void SaveGutschrift(Gutschrift gs)
        {
            persistenceService.Save(gs);
        }

        public Frachtabrechnung GetFrachtabrechnungById(int fabNr)
        {
            return persistenceService.GetById<Frachtabrechnung, int>(fabNr);
        }

        public void SpeichereKundenrechnung(Kundenrechnung kr)
        {
            persistenceService.Save(kr);
        }

        public Kundenrechnung GetKundenrechnungById(int krNr)
        {
            return persistenceService.GetById<Kundenrechnung, int>(krNr);
        }

        public IList<KundenrechnungDTO> GetKundenrechnungen()
        {
            var lsa =
                (from sa in persistenceService.Query<Kundenrechnung>()
                 select sa).ToList();
            IList<KundenrechnungDTO> krdto = new List<KundenrechnungDTO>();
            foreach (var l in lsa)
            {
                krdto.Add(l.ToDTO());
            }
            return krdto;            
        }

        public void SpeichereZahlungseingang(Zahlungseingang ze)
        {
            persistenceService.Save(ze);
        }
    }
}
