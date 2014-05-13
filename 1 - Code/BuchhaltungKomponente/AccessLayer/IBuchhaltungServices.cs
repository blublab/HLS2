using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.UnterbeauftragungKomponente.AccessLayer;

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

        void SetzeUnterbeauftragungServices(IUnterbeauftragungServicesFuerBuchhaltung unterbeauftragungServices);
    }
}
