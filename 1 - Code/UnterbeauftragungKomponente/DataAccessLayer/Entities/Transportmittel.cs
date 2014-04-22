using FluentNHibernate.Mapping;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Util.Common.DataTypes;
using Util.Common.Interfaces;

namespace ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer
{
    public enum TransportmittelArt
    {
        LKW, Containerschiff, Frachtflugzeug
    }

    public enum GeschwindigkeitArt
    {
        Niedrig, Mittel, Hoch
    }

    public class Transportmittel : ICanConvertToDTO<TransportmittelDTO>
    {
        public virtual int TmNr { get; set; }
        public virtual TransportmittelArt TmArt { get; set; }
        public virtual GeschwindigkeitArt Geschwindigkeit { get; set; }
        public virtual int KapazitaetInTEU { get; set; }
        public virtual WaehrungsType Fixkosten { get; set; }
        public virtual WaehrungsType Entfernungskosten { get; set; }
        public virtual WaehrungsType Dauerkosten { get; set; }
        public virtual WaehrungsType MengenkostenProTEU { get; set; }
        public virtual WaehrungsType MengenkostenProFEU { get; set; }

        public virtual TransportmittelDTO ToDTO()
        {
            TransportmittelDTO tmDTO = new TransportmittelDTO();
            tmDTO.TmNr = this.TmNr;
            tmDTO.TmArt = this.TmArt;
            tmDTO.Geschwindigkeit = this.Geschwindigkeit;
            tmDTO.KapazitaetInTEU = this.KapazitaetInTEU;
            tmDTO.Fixkosten = this.Fixkosten;
            tmDTO.Entfernungskosten = this.Entfernungskosten;
            tmDTO.Dauerkosten = this.Dauerkosten;
            tmDTO.MengenkostenProTEU = this.MengenkostenProTEU;
            tmDTO.MengenkostenProFEU = this.MengenkostenProFEU;
            return tmDTO;
        }

        internal class TransportmittelMap : ClassMap<Transportmittel>
        {
            public TransportmittelMap()
            {
                this.Id(x => x.TmNr);
                this.Map(x => x.TmArt);
                this.Map(x => x.Geschwindigkeit);
                this.Map(x => x.KapazitaetInTEU);
                this.Map(x => x.Fixkosten);
                this.Map(x => x.Entfernungskosten);
                this.Map(x => x.Dauerkosten);
                this.Map(x => x.MengenkostenProTEU);
                this.Map(x => x.MengenkostenProFEU);
            }
        }
    }
}
