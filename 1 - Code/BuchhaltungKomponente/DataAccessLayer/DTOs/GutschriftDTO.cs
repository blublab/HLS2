using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.DataTypes;
using Util.Common.Interfaces;

namespace ApplicationCore.BuchhaltungKomponente.DataAccessLayer
{
    public class GutschriftDTO : ICanConvertToEntity<Gutschrift>
    {
        public virtual int GutSchrNr { get; set; }
        public virtual KontodatenType Kontodaten { get; set; }
        public virtual WaehrungsType Betrag { get; set; }

        public virtual Gutschrift ToEntity()
        {
            Gutschrift gutSchr = new Gutschrift();
            gutSchr.GutSchrNr = this.GutSchrNr;
            gutSchr.Kontodaten = this.Kontodaten;
            gutSchr.Betrag = this.Betrag;
            return gutSchr;
        }
    }
}
