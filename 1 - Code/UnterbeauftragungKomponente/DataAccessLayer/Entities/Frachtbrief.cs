using System;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using FluentNHibernate.Mapping;
using Util.Common.DataTypes;
using Util.Common.Interfaces;

namespace ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer
{
    public class Frachtbrief : ICanConvertToDTO<FrachtbriefDTO>
    {
        public virtual int FrbNr { get; set; }
        public virtual DateTime AusstellungsZeit { get; set; }
        public virtual string AbsenderName { get; set; }
        public virtual string FrachtfuehrerName { get; set; }
        public virtual string EmpfaengerName { get; set; }
        public virtual Adresse AbsenderAnschrift { get; set; }
        public virtual Adresse FrachtfuehrerAnschrift { get; set; }
        public virtual Adresse EmpfaengerAnschrift { get; set; }
        public virtual string UebernahmeOrt { get; set; }
        public virtual DateTime UebernahmeZeit { get; set; }
        public virtual string Warenbezeichnung { get; set; }
        public virtual decimal Rohgewicht { get; set; }
        public virtual WaehrungsType Kosten { get; set; }
        public virtual string FrachtzahlungVermerk { get; set; }

        public virtual FrachtbriefDTO ToDTO()
        {
            return new FrachtbriefDTO
            {
                FrbNr = this.FrbNr,
                AusstellungsZeit = this.AusstellungsZeit,
                AbsenderName = this.AbsenderName,
                FrachtfuehrerName = this.FrachtfuehrerName,
                EmpfaengerName = this.EmpfaengerName,
                AbsenderAnschrift = this.AbsenderAnschrift != null ?
                    this.AbsenderAnschrift.ToDTO() : null,
                FrachtfuehrerAnschrift = this.FrachtfuehrerAnschrift != null ?
                    this.FrachtfuehrerAnschrift.ToDTO() : null,
                EmpfaengerAnschrift = this.EmpfaengerAnschrift != null ?
                    this.EmpfaengerAnschrift.ToDTO() : null,
                UebernahmeOrt = this.UebernahmeOrt,
                UebernahmeZeit = this.UebernahmeZeit,
                Warenbezeichnung = this.Warenbezeichnung,
                Rohgewicht = this.Rohgewicht,
                Kosten = this.Kosten,
                FrachtzahlungVermerk = this.FrachtzahlungVermerk
            };
        }
    }

    internal class FrachtbriefMap : ClassMap<Frachtbrief>
    {
        public FrachtbriefMap()
        {
            this.Id(x => x.FrbNr);
            this.Map(x => x.AusstellungsZeit);
            this.Map(x => x.AbsenderName);
            this.Map(x => x.FrachtfuehrerName);
            this.Map(x => x.EmpfaengerName);

//           this.References(x => x.AbsenderAnschrift);
//            this.References(x => x.FrachtfuehrerAnschrift);
//            this.References(x => x.EmpfaengerAnschrift);
            this.Map(x => x.UebernahmeOrt);
            this.Map(x => x.UebernahmeZeit);
            this.Map(x => x.Warenbezeichnung);
            this.Map(x => x.Rohgewicht);
            this.Map(x => x.Kosten);
            this.Map(x => x.FrachtzahlungVermerk);
        }
    }
}
