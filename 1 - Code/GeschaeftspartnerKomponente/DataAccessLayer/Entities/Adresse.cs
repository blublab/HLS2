using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.Interfaces;

namespace ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer
{
    public class Adresse : ICanConvertToDTO<AdresseDTO>
    {
        public virtual int Id { get; set; }
        public virtual string Strasse { get; set; }
        public virtual string Hausnummer { get; set; }
        public virtual string PLZ { get; set; }
        public virtual string Wohnort { get; set; }
        public virtual string Land { get; set; }

        public Adresse() 
        {
        }

        public virtual AdresseDTO ToDTO()
        {
            AdresseDTO adDTO = new AdresseDTO();
            adDTO.Id = this.Id;
            adDTO.Strasse = this.Strasse;
            adDTO.Hausnummer = this.Hausnummer;
            adDTO.PLZ = this.PLZ;
            adDTO.Wohnort = this.Wohnort;
            adDTO.Land = this.Land;
            return adDTO;
        }
    }

    internal class AdresseMap : ClassMap<Adresse>
    {
        public AdresseMap()
        {
            this.Id(x => x.Id);

            this.Map(x => x.Strasse).Not.Nullable();
            this.Map(x => x.Hausnummer).Not.Nullable();
            this.Map(x => x.PLZ).Not.Nullable();
            this.Map(x => x.Wohnort).Not.Nullable();
            this.Map(x => x.Land).Not.Nullable();
        }
    }
}
