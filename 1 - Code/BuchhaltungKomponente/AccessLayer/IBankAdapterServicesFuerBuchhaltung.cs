using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.BuchhaltungKomponente.AccessLayer
{
    public interface IBankAdapterServicesFuerBuchhaltung
    {
        /// <summary>
        /// Sendet Gutschrift an Bank
        /// </summary>
        /// <param name="gDTO">Gutschrift DTO</param>
        /// <throws>Argument Exception wenn gDTO == null </throws>
        void SendeGutschriftAnBank(ref GutschriftDTO gDTO);
    }
}
