﻿using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.DataTypes;
using Util.Common.Interfaces;

namespace ApplicationCore.BuchhaltungKomponente.DataAccessLayer
{
    public class Kundenrechnung : ICanConvertToDTO<KundenrechnungDTO>
    {
        public virtual int RechnungsNr { get; set; }
        public virtual WaehrungsType Rechnungsbetrag { get; set; }
        public virtual bool RechnungBezahlt { get; set; }
        public virtual int Sendungsanfrage { get; set; }
        public virtual int Rechnungsadresse { get; set; }

        public Kundenrechnung()
        {
        }

        public virtual KundenrechnungDTO ToDTO()
        {
            KundenrechnungDTO krDTO = new KundenrechnungDTO();
            krDTO.RechnungsNr = this.RechnungsNr;
            krDTO.Rechnungsbetrag = this.Rechnungsbetrag;
            krDTO.RechnungBezahlt = this.RechnungBezahlt;
            krDTO.Sendungsanfrage = this.Sendungsanfrage;
            krDTO.Rechnungsadresse = Rechnungsadresse;
            return krDTO;
        }

        internal class KundenrechnungMap : ClassMap<Kundenrechnung>
        {
            public KundenrechnungMap()
            {
                this.Id(x => x.RechnungsNr);
                this.Map(x => x.Rechnungsbetrag).Not.Nullable();
                this.Map(x => x.RechnungBezahlt).Not.Nullable();
                this.Map(x => x.Sendungsanfrage).Not.Nullable();
                this.Map(x => x.Rechnungsadresse);
            }
        }
    }
}
