using System;
using ApplicationCore.GeschaeftspartnerKomponente.DataAccessLayer;
using Util.Common.DataTypes;
using Util.Common.Interfaces;

namespace ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer
{
    public class FrachtbriefDTO : DTOType<FrachtbriefDTO>, ICanConvertToEntity<Frachtbrief>
    {
        public virtual int FrbNr { get; set; }
        public virtual DateTime AusstellungsZeit { get; set; }
        public virtual string AbsenderName { get; set; }
        public virtual string FrachtfuehrerName { get; set; }
        public virtual string EmpfaengerName { get; set; }
        public virtual AdresseDTO AbsenderAnschrift { get; set; }
        public virtual AdresseDTO FrachtfuehrerAnschrift { get; set; }
        public virtual AdresseDTO EmpfaengerAnschrift { get; set; }
        public virtual string UebernahmeOrt { get; set; }
        public virtual DateTime UebernahmeZeit { get; set; }
        public virtual string Warenbezeichnung { get; set; }
        public virtual decimal Rohgewicht { get; set; }
        public virtual WaehrungsType Kosten { get; set; }
        public virtual string FrachtzahlungVermerk { get; set; }


        public virtual Frachtbrief ToEntity()
        {
            return new Frachtbrief
            {
                FrbNr = this.FrbNr,
                AusstellungsZeit = this.AusstellungsZeit,
                AbsenderName = this.AbsenderName,
                FrachtfuehrerName = this.FrachtfuehrerName,
                EmpfaengerName = this.EmpfaengerName,
                AbsenderAnschrift = this.AbsenderAnschrift != null ?
                    this.AbsenderAnschrift.ToEntity() : null,
                FrachtfuehrerAnschrift = this.FrachtfuehrerAnschrift != null ?
                    this.FrachtfuehrerAnschrift.ToEntity() : null,
                EmpfaengerAnschrift = this.EmpfaengerAnschrift != null ?
                this.EmpfaengerAnschrift.ToEntity() : null,
                UebernahmeOrt = this.UebernahmeOrt,
                UebernahmeZeit = this.UebernahmeZeit,
                Warenbezeichnung = this.Warenbezeichnung,
                Rohgewicht = this.Rohgewicht,
                Kosten = this.Kosten,
                FrachtzahlungVermerk = this.FrachtzahlungVermerk
            };
        }
    }
}
