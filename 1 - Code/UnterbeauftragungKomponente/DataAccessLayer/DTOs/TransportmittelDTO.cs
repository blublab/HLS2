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
    public class TransportmittelDTO : ICanConvertToEntity<Transportmittel>
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

        public virtual Transportmittel ToEntity()
        {
            Transportmittel tm = new Transportmittel();
            tm.TmNr = this.TmNr;
            tm.TmArt = this.TmArt;
            tm.Geschwindigkeit = this.Geschwindigkeit;
            tm.KapazitaetInTEU = this.KapazitaetInTEU;
            tm.Fixkosten = this.Fixkosten;
            tm.Entfernungskosten = this.Entfernungskosten;
            tm.Dauerkosten = this.Dauerkosten;
            tm.MengenkostenProTEU = this.MengenkostenProTEU;
            tm.MengenkostenProFEU = this.MengenkostenProFEU;
            return tm;
        }
    }
}
