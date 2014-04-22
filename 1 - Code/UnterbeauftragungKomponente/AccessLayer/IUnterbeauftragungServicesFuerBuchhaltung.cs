﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;

namespace ApplicationCore.UnterbeauftragungKomponente.AccessLayer
{
    public interface IUnterbeauftragungServicesFuerBuchhaltung
    {
        /// <summary>
        /// Setzt Frachtauftrag auf "abgeschlossen".
        /// </summary>
        /// <throws>ArgumentExeption, falls faufNr < 0</throws>
        void SchliesseFrachtauftragAb(int faufNr);

        /// <summary>
        /// Prüft, ob der Frachtauftrag existiert.
        /// </summary>
        /// <param name="faufNr">ID des Frachtauftrages</param>
        /// <returns>true, falls vorhanden, sonst false</returns>
        bool PruefeObFrachtauftragVorhanden(int faufNr);
    }
}
