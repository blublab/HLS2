using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.DataTypes;
using Util.Common.Interfaces;

namespace ApplicationCore.BuchhaltungKomponente.DataAccessLayer
{
    public class Frachtabrechnung : ICanConvertToDTO<FrachtabrechnungDTO>
    {
        public virtual int FabNr { get; set; }
        public virtual bool IstBestaetigt { get; set; }
        public virtual WaehrungsType Rechnungsbetrag { get; set; }
        public virtual int FaufNr { get; set; }
        public virtual Gutschrift Gutschrift { get; set; }
        public virtual int RechnungsNr { get; set; }
        ////public virtual PDFTyp Inhalt {get;set;}

        public Frachtabrechnung()
        {
            this.IstBestaetigt = false;
            this.Rechnungsbetrag = new WaehrungsType(0);
            this.FaufNr = 0;
            this.RechnungsNr = 0;
        }

        public virtual FrachtabrechnungDTO ToDTO()
        {
            FrachtabrechnungDTO fabDTO = new FrachtabrechnungDTO();
            fabDTO.FabNr = this.FabNr;
            fabDTO.IstBestaetigt = this.IstBestaetigt;
            fabDTO.Rechnungsbetrag = this.Rechnungsbetrag;
            fabDTO.FaufNr = this.FaufNr;
            fabDTO.Gutschrift = this.Gutschrift;
            fabDTO.RechnungsNr = this.RechnungsNr;
            ////fabDTO.Inhalt = this.Inhalt;
            return fabDTO;
        }
    }

    internal class FrachtabrechnungMap : ClassMap<Frachtabrechnung>
    {
        public FrachtabrechnungMap()
        {
            this.Id(x => x.FabNr);

            this.Map(x => x.IstBestaetigt).Not.Nullable();
            this.Map(x => x.Rechnungsbetrag).Not.Nullable();
            this.Map(x => x.FaufNr).Not.Nullable();
            this.Map(x => x.RechnungsNr).Not.Nullable();
            this.References(x => x.Gutschrift).Cascade.All();
            ////this.Map(x => x.Inhalt)
        }
    }
}
