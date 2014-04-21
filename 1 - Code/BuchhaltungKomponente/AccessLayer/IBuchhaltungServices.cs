using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;

namespace ApplicationCore.BuchhaltungKomponente.AccessLayer
{
    public interface IBuchhaltungServices
    {
        // TODO in Unterbeauftragungskomponente auf Frachtauftragsnummer pruefen
        void createFrachtabrechnung(ref FrachtabrechnungDTO faDTO);

        //DTO oder Nummer??
        void bezahleFrachtabrechnung(ref FrachtabrechnungDTO faDTO);

        //DTO oder Nummer??
        void deleteFrachtabrechnung(int faNr);

        FrachtabrechnungDTO findFrachtabrechnung(int faNr);


    }
}
