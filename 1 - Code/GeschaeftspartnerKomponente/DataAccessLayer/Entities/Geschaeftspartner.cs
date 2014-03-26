using Common.DataTypes;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.Interfaces;

namespace ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer
{
    public class Geschaeftspartner : ICanConvertToDTO<GeschaeftspartnerDTO>
    {
        public virtual int GpNr { get; set; }
        public virtual string Nachname { get; set; }
        public virtual string Vorname  { get; set; }
        public virtual long Version { get; set; }
        public virtual EMailType Email { get; set; }

        public Geschaeftspartner()
        {
        }

        public virtual GeschaeftspartnerDTO ToDTO()
        {
            GeschaeftspartnerDTO gpDTO = new GeschaeftspartnerDTO();
            gpDTO.GpNr = this.GpNr;
            gpDTO.Vorname = this.Vorname;
            gpDTO.Nachname = this.Nachname;
            gpDTO.Version = this.Version;
            gpDTO.Email = this.Email;
            return gpDTO;
        }
    }

    internal class GeschaeftspartnerMap : ClassMap<Geschaeftspartner>
    {
        public GeschaeftspartnerMap()
        {
            this.Id(x => x.GpNr);

            this.Map(x => x.Nachname).Not.Nullable();
            this.Map(x => x.Vorname).Not.Nullable();
            this.Version(x => x.Version);
            this.Map(x => x.Email).Not.Nullable();
        }
    }
}
