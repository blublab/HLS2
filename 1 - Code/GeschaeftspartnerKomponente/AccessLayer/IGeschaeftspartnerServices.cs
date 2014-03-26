using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
 
namespace ApplicationCore.GeschaeftspartnerKomponente.AccessLayer
{
    public interface IGeschaeftspartnerServices
    {
        /// <summary>
        /// Erzeugt eine Geschaeftspartner Entität.
        /// </summary>
        /// <throws>ArgumentException, falls gpDTO == null.</throws>
        /// <throws>ArgumentException, falls gpDTO.GpNr != 0.</throws>
        /// <transaction>Nicht erlaubt</transaction>
        void CreateGeschaeftspartner(ref GeschaeftspartnerDTO gpDTO);

        /// <summary>
        /// Ändert eine Geschaeftspartner Entität.
        /// </summary>
        /// <throws>ArgumentException, falls gpDTO == null.</throws>
        /// <throws>GeschaeftspartnerNichtGefundenException</throws>
        /// <transaction>Nicht erlaubt</transaction>
        void UpdateGeschaeftspartner(ref GeschaeftspartnerDTO gpDTO);

        /// <summary>
        /// Sucht einen Geschaeftspartner nach Id.
        /// </summary>
        /// <throws>ArgumentException, falls gpNr <= 0.</throws>
        /// <returns>Geschaeftspartner; null, falls nicht gefunden.</returns>
        /// <transaction>Optional</transaction>
        GeschaeftspartnerDTO FindGeschaeftspartner(int gpNr);

        /// <summary>
        /// Speichert eine Adresse.
        /// </summary>
        /// <throws>ArgumentException, falls Id <= 0.</throws>
        /// <returns>Adresse; null, falls nicht gefunden.</returns>
        /// <transaction>Nicht erlaubt</transaction>
        void CreateAdresse(ref AdresseDTO adDTO);

        /// <summary>
        /// Sucht eine Adresse nach Id.
        /// </summary>
        /// <throws>ArgumentException, falls Id <= 0.</throws>
        /// <returns>Adresse; null, falls nicht gefunden.</returns>
        /// <transaction>Optional</transaction>
        AdresseDTO FindAdresse(int adId);
    }
}
