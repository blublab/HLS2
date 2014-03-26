using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
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
    public class GeschaeftspartnerDTO : DTOType<GeschaeftspartnerDTO>, ICanConvertToEntity<Geschaeftspartner>
    {
        public int GpNr { get; set; }
        public string Nachname { get; set; }
        public string Vorname  { get; set; }
        public long Version { get; set; }
        public EMailType Email { get; set; }

        public virtual Geschaeftspartner ToEntity()
        {
            Geschaeftspartner gp = new Geschaeftspartner();
            gp.GpNr = this.GpNr;
            gp.Vorname = this.Vorname;
            gp.Nachname = this.Nachname;
            gp.Version = this.Version;
            gp.Email = this.Email;
            return gp;
        }
    }
}
