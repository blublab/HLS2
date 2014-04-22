using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;

namespace ApplicationCore.BuchhaltungKomponente.AccessLayer
{
    public abstract class BuchhaltungKomponenteExceptions : Exception
    {
        public int FabNr { get; private set; }

        public BuchhaltungKomponenteExceptions(int fabNr)
        {
            this.FabNr = fabNr;
        }

        public abstract string Meldung { get; }
    }

    public class FrachtauftragNichtGefundenException : BuchhaltungKomponenteExceptions
    {
        public FrachtauftragNichtGefundenException(int faufNr)
            : base(faufNr)
        {
        }

        public override string Meldung
        {
            get
            {
                return "Die Frachtabrechnung konnte nicht erstellt werden, da der Frachtauftrag " + this.FabNr + " nicht gefunden wurde.";
            }
        }
    }
}
