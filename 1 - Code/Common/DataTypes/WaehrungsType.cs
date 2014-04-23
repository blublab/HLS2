using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Common.Interfaces;

namespace Util.Common.DataTypes
{
    [Serializable]
    public class WaehrungsType : ValueType<WaehrungsType>
    {
        public enum Waehrungen : decimal {
            Euro = 1,
        }
        public readonly decimal Wert;

        public WaehrungsType(decimal wert)
        {
            this.Wert = wert;
        }

        public static WaehrungsType operator +(WaehrungsType wt1, WaehrungsType wt2)
        {
            return new WaehrungsType(wt1.Wert + wt2.Wert);
        }

        public static WaehrungsType operator -(WaehrungsType w1, WaehrungsType w2)
        {
            return new WaehrungsType(w1.Wert - w2.Wert);
        }

        public static WaehrungsType operator /(WaehrungsType wt1, WaehrungsType wt2)
        {
            return new WaehrungsType(wt1.Wert / wt2.Wert);
        }

        public static WaehrungsType operator *(WaehrungsType w1, WaehrungsType w2)
        {
            return new WaehrungsType(w1.Wert * w2.Wert);
        }

        public decimal ToYen()
        {
            decimal umrechnungsfaktor = 141.70m; //conversion ratio from 22.10.2013 20:00
            return Wert * umrechnungsfaktor;
        }

        public decimal ToDollar()
        {
            decimal umrechnungsfaktor = 0.725505m; //conversion ratio from 22.10.2013 20:00
            return Wert * umrechnungsfaktor;
        }

        public override bool Equals(object obj)
        {
            WaehrungsType other = obj as WaehrungsType;

            if (other == null)
            {
                return false;
            }

            return this.Wert.Equals(other.Wert);
        }

        public override int GetHashCode()
        {
            return this.Wert.GetHashCode();
        }

        public override string ToString()
        {
            return this.Wert.ToString();
        }
    }
}
