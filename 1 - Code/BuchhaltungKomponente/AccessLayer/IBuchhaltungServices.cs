using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;

namespace ApplicationCore.BuchhaltungKomponente.AccessLayer
{
    public interface IBuchhaltungServices
    {
        /// <summary>
        /// Erzeugt eine Frachtabrechnung Entität.
        /// </summary>
        /// <throws>ArgumenException, falls FrachtauftragDTO == null.</throws>
        /// <throws>FrachtauftragNichtGefundenException, falls Frachtauftrag nicht exsistiert.</throws>
        /// <transaction>Nicht erlaubt</transaction>
        FrachtabrechnungDTO CreateFrachtabrechnung(int faufNr);

        /// <summary>
        /// Estellt Gutschrift und schliesst Frachtauftrag ab.
        /// </summary>
        /// <throws>ArgumentException, falls FrachtauftragDTO == null</throws>
        /// <post>Frachtauftrag befindet sich im Zustand "Abgeschlossen".</post>
        void PayFrachtabrechnung(ref FrachtabrechnungDTO fabDTO);

        /// <summary>
        /// Löscht Frachtabrechnung und ggf. Gutschrift.
        /// </summary>
        /// <throws>ArgumentException, falls FrachtauftragDTO == null</throws>
        void DeleteFrachtabrechnung(ref FrachtabrechnungDTO fabDTO);

        /// <summary>
        /// Liest der Frachtabrechnung aus der DB.
        /// </summary>
        /// <param name="fabNr">ID der Frachtabrechnung</param>
        /// <returns>DTO der Frachtabrechnung</returns>
        /// <throws>ArgumentException, falls fabNr <= 0</throws>
        /// <transaction>Nicht erlaubt</transaction>
        FrachtabrechnungDTO ReadFrachtabrechnungByID(int fabNr);
    }
}
