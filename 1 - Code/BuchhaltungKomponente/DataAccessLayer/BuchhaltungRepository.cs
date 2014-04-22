using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
