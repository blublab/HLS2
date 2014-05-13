using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.BuchhaltungKomponente.AccessLayer
{
    public interface IBuchhaltungServicesFuerFrachtfuehrerAdapter
    {
        /// <summary>
        /// Estellt Gutschrift und schliesst Frachtauftrag ab.
        /// </summary>
        /// <throws>ArgumentException, falls FrachtauftragDTO == null</throws>
        /// <post>Frachtauftrag befindet sich im Zustand "Abgeschlossen".</post>
        void PayFrachtabrechnung(ref FrachtabrechnungDTO fabDTO);

        void SetzeUnterbeauftragungServices(IUnterbeauftragungServicesFuerBuchhaltung unterbeauftragungServices);
    }
}
