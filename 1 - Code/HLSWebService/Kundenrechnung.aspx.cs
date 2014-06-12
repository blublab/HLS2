using System;
using System.Collections.Generic;
using System.Web.UI;
using Newtonsoft.Json;

namespace HLSWebService
{
    /// <summary>
    /// Code Behind.
    /// </summary>
    public partial class Kundenrechnung : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int krNr = int.Parse(RouteData.Values["krNr"].ToString());
            HLS hls = Application["HLS"] as HLS;

            IList<object> rechnungen = new List<object>();
            foreach (var af in hls.GetKundenrechnungen(krNr))
            {
                var anon = new
                {
                    RnNr = af.RechnungsNr,
                    Bezahlt = af.RechnungBezahlt,
                    Betrag = af.Rechnungsbetrag,
                    SaNr = af.Sendungsanfrage
                };
                rechnungen.Add(anon);
            }
            string json = JsonConvert.SerializeObject(rechnungen);
            Response.Write(json);
        }
    }
}