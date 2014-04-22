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
    public class Gutschrift : ICanConvertToDTO<GutschriftDTO>
    {
        public virtual int GutSchrNr { get; set; }
        public virtual KontodatenType Kontodaten { get; set; }
        public virtual WaehrungsType Betrag { get; set; }

        public virtual GutschriftDTO ToDTO()
        {
            GutschriftDTO gutschrDTO = new GutschriftDTO();
            gutschrDTO.GutSchrNr = this.GutSchrNr;
            gutschrDTO.Kontodaten = this.Kontodaten;
            gutschrDTO.Betrag = this.Betrag;
            return gutschrDTO;
        }
    }

    internal class GutschriftMap : ClassMap<Gutschrift>
    {
        public GutschriftMap()
        {
            this.Id(x => x.GutSchrNr);

            this.Map(x => x.Kontodaten);
            this.Map(x => x.Betrag);
        }
    }
}
