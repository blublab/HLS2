﻿using Util.Common.DataTypes;
using Util.Common.Interfaces;

namespace ApplicationCore.BuchhaltungKomponente.DataAccessLayer
{
    public class KundenrechnungDTO : DTOType<KundenrechnungDTO>, ICanConvertToEntity<Kundenrechnung>
    {
        public virtual int RechnungsNr { get; set; }
        public virtual WaehrungsType Rechnungsbetrag { get; set; }
        public virtual bool RechnungBezahlt { get; set; }
        public virtual int Sendungsanfrage { get; set; }
        public virtual int Rechnungsadresse { get; set; }

        public KundenrechnungDTO()
        {
        }

        public virtual Kundenrechnung ToEntity()
        {
            Kundenrechnung kr = new Kundenrechnung();
            kr.RechnungsNr = this.RechnungsNr;
            kr.Rechnungsbetrag = this.Rechnungsbetrag;
            kr.RechnungBezahlt = this.RechnungBezahlt;
            kr.Sendungsanfrage = this.Sendungsanfrage;
            kr.Rechnungsadresse = this.Rechnungsadresse;
            return kr;
        }
    }
}
