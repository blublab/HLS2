using ApplicationCore.BankAdapter.BusinessLogicLayer;
using ApplicationCore.BuchhaltungKomponente.AccessLayer;
using ApplicationCore.BuchhaltungKomponente.DataAccessLayer;
using Common.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.BankAdapter.AccessLayer
{
    public class BankAdapterFacade : IBankAdapterServicesFuerBuchhaltung
    {
        private readonly BankAdapterBusinessLogic ba_BL;

        public BankAdapterFacade()
        {
            this.ba_BL = new BankAdapterBusinessLogic();
        }
        public void SendeGutschriftAnBank(ref GutschriftDTO gDTO)
        {
            Check.Argument(gDTO != null, "gDTO != null");

            this.ba_BL.SendeGutschriftAnBank(gDTO);
        }
    }
}
