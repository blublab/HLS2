using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.DataTypes;
using Util.Common.Interfaces;

namespace ApplicationCore.BuchhaltungKomponente.DataAccessLayer
{
    public class FrachtabrechnungDTO : ICanConvertToEntity<Frachtabrechnung>
    {
        public virtual int FabNr { get; set; }
        public virtual bool IstBestaetigt { get; set; }
        public virtual WaehrungsType Rechnungsbetrag { get; set; }
        public virtual int FaufNr { get; set; }
        public virtual Gutschrift Gutschrift { get; set; }
        ////public virtual PDFTyp Inhalt {get;set;}

        public virtual Frachtabrechnung ToEntity() 
        {
            Frachtabrechnung fab = new Frachtabrechnung();
            fab.FabNr = this.FabNr;
            fab.IstBestaetigt = this.IstBestaetigt;
            fab.Rechnungsbetrag = this.Rechnungsbetrag;
            fab.FaufNr = this.FaufNr;
            fab.Gutschrift = this.Gutschrift;
            ////fab.Inhalte = this.Inhalt
            return fab;
        }
    }
}
