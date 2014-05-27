using ApplicationCore.AuftragKomponente.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.AuftragKomponente.AccessLayer
{
    public interface IUnterbeauftragungServicesFuerAuftrag
    {
        /// <summary>
        /// Erstellt einen Frachtbrief und vesendet ihn an den Frachtfüherer.
        /// </summary>
        void ErstelleFrachtbriefUndVerschickeIhn(SendungsanfrageDTO saDTO);
    }
}
