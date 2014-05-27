﻿using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.FrachtfuehrerAdapter.BusinessLogicLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using Common.Implementations;

namespace ApplicationCore.FrachtfuehrerAdapter.AccessLayer
{
    public class FrachtfuehrerAdapterFacade : IFrachtfuehrerServicesFürUnterbeauftragung
    {
        private readonly FrachtfuehrerAdapterBusinessLogic ffA_BL;

        public FrachtfuehrerAdapterFacade(ref IBuchhaltungServicesFuerFrachtfuehrerAdapter buchhaltungServices)
        {
            this.ffA_BL = new FrachtfuehrerAdapterBusinessLogic(buchhaltungServices);
        }

        public void SendeFrachtauftragAnFrachtfuehrer(FrachtauftragDTO fraDTO)
        {
            Check.Argument(fraDTO != null, "fraDTO != null");

            this.ffA_BL.SendeFrachtauftragAnFrachtfuehrer(fraDTO);
        }

        public void StarteEmpfangVonFrachtabrechnungen()
        {
            this.ffA_BL.StarteEmpfangVonFrachtabrechnungen();
        }
    }
}
