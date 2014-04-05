using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.Interfaces;

namespace ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer
{
    public class AdresseDTO : DTOType<AdresseDTO>, ICanConvertToEntity<Adresse>
    {
        public virtual int Id { get; set; }
        public virtual string Strasse { get; set; }
        public virtual string Hausnummer { get; set; }
        public virtual string PLZ { get; set; }
        public virtual string Wohnort { get; set; }
        public virtual string Land { get; set; }

        public virtual Adresse ToEntity()
        {
            Adresse ad = new Adresse();
            ad.Id = this.Id;
            ad.Strasse = this.Strasse;
            ad.Hausnummer = this.Hausnummer;
            ad.PLZ = this.PLZ;
            ad.Wohnort = this.Wohnort;
            ad.Land = this.Land;
            return ad;
        }
    }
}
