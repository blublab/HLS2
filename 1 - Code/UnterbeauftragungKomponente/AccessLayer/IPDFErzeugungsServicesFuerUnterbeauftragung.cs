using ApplicationCore.UnterbeauftragungKomponente.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.UnterbeauftragungKomponente.AccessLayer
{
    public interface IPDFErzeugungsServicesFuerUnterbeauftragung
    {
        string ErzeugeFrachtbriefPDF(FrachtbriefDTO fbDTO);
    }
}
