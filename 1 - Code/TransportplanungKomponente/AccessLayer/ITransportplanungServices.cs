using ApplicationCore.AuftragKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationCore.TransportplanungKomponente.AccessLayer
{
    public interface ITransportplanungServices
    {
        /// <summary>
        /// Liefert eine Liste der erzeugten Transportpläne, falls verfügbar.
        /// </summary>
        /// <returns>Transportpläne; leere Liste, falls keine Pläne vorhanden.</returns>
        /// <throws>ArgumentException, falls saNr <= 0.</throws>
        /// <throws>SendungsanfrageNichtGefundenException</throws>
        /// <transaction>Optional</transaction>
        List<TransportplanDTO> FindTransportplaeneZuSendungsanfrage(int saNr);

        /// <summary>
        /// Wählt einen Transportplan aus und beauftragt dessen Auführung an den Frachtführer.
        /// Alle alternativen Transportpläne werden verworfen.
        /// </summary>
        /// <throws>ArgumentException, falls tpNr <= 0.</throws>
        /// <throws>TransportplanNichtGefundenException</throws>
        /// <throws>SendungsanfrageNichtGefundenException, falls zu Transportplan zugeordnete Sendungsanfrage nicht gefunden wurde.</throws>
        /// <throws>SendungsanfrageNichtAngenommenException, falls zu Transportplan zugeordnete Sendungsanfrage nicht angenommen wurde.</throws>
        /// <transaction>Nicht Erlaubt</transaction>
        /// <post>Zugehörige Sendungsanfrage befindet sich im Zustand "In Ausführung".</post>
        void FühreTransportplanAus(int tpNr);
    }
}
