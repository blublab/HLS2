using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuchhaltungKomponente.BusinessLogicLayer
{
    internal class BuchhaltungKomponenteBusinessLogic
    {
        private IUnterbeauftragungServicesFuerBuchhaltung unterbeauftragungService;

        internal BuchhaltungKomponenteBusinessLogic()
        {
        }

        internal bool PruefeObFrachtauftragVorhanden(int faufNr)
        {
            return unterbeauftragungService.PruefeObFrachtauftragVorhanden(faufNr);
        }

        internal void SchliesseFrachtauftragAb(int faufNr)
        {
            unterbeauftragungService.SchliesseFrachtauftragAb(faufNr);
        }

        internal void setUnterbeauftragungServices(IUnterbeauftragungServicesFuerBuchhaltung unterbeauftragungServices)
        {
            this.unterbeauftragungService = unterbeauftragungServices;
        }
    }
}
