using System;
using System.Collections.Generic;
using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;

namespace ApplicationCore.UnterbeauftragungKomponente.AccessLayer
{
    public interface IUnterbeauftragungServices
    {
        /// <summary>
        /// Legt einen neuen FrachtführerRahmenvertrag an.
        /// </summary>
        /// <throws>ArgumentException, falls frvDTO == null.</throws>
        /// <throws>ArgumentException, falls frvDTO.FrvNr != 0.</throws>
        /// <transaction>Nicht erlaubt</transaction>
        void CreateFrachtfuehrerRahmenvertrag(ref FrachtfuehrerRahmenvertragDTO frvDTO);

        /// <summary>
        /// Legt einen neuen Frachtführer an.
        /// </summary>
        /// <throws>ArgumentException, falls frfDTO == null.</throws>
        /// <throws>ArgumentException, falls frfDTO.FrfNr != 0.</throws>
        /// <transaction>Nicht erlaubt</transaction>
        void CreateFrachtfuehrer(ref FrachtfuehrerDTO frfDTO);

        /// <summary>
        /// Erstellt Frachtauftrag.
        /// </summary>
        /// <throws>ArgumentException, falls frfDTO == null.</throws>
        /// <throws>ArgumentException, falls frfDTO.FrfNr != 0.</throws>
        /// <transaction>Nicht erlaubt</transaction>        
        void CreateFrachtauftrag(ref FrachtauftragDTO faufDTO);

        /// <summary>
        /// Liest der Frachtauftrag aus der DB.
        /// </summary>
        /// <param name="fabNr">ID des Fracahtauftrages</param>
        /// <throws>ArgumentException, falls faufNr <= 0</throws>
        /// <transaction>Nicht erlaubt</transaction>
        FrachtauftragDTO readFrachtauftragByID(int faufNr);
    }
}
