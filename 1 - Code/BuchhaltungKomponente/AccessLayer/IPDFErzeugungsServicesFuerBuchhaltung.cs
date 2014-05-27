using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using ApplicationCore.TransportplanungKomponente.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.BuchhaltungKomponente.AccessLayer
{
    public interface IPDFErzeugungsServicesFuerBuchhaltung
    {
        /// <summary>
        /// Erstellt eine PDF Datei zu einer Kundenrechnung.
        /// </summary>
        /// <throws>ArgumentException, falls krDTO == null.</throws>
        string ErstelleKundenrechnungPDF(KundenrechnungDTO krDTO, IList<TransportplanSchrittDTO> tpSchritte, GeschaeftspartnerDTO gpDTO);
    }
}
