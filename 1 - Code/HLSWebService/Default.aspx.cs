using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using log4net;
using Newtonsoft.Json;

namespace HLSWebService
{
    /// <summary>
    /// Code Behind.
    /// </summary>
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            long saNr = Request.Params["SaNr"] != null ? long.Parse(Request.Params["SaNr"]) : -1;
            
            if (Application["HLS"] == null)
            {
                Application["HLS"] = new HLS();
            }
            HLS hls = Application["HLS"] as HLS;

            IList<object> anfragen = new List<object>();
            foreach (var af in hls.GetSendungsanfragen(saNr))
            {
                var anon = new
                {
                    SaNr = af.SaNr,
                    Start = hls.FindLokation(af.StartLokation),
                    Ziel = hls.FindLokation(af.ZielLokation),
                    Status = af.Status.ToString(),
                    Auftrageber = hls.FindGeschaeftspartner(af.AuftrageberNr)
                };
                anfragen.Add(anon);
            }
            string json = JsonConvert.SerializeObject(anfragen);
            Response.Write(json);
        }
    }
}