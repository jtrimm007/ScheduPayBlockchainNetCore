using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduPayBlockchainNetCore.Models
{
    public class InvoiceDetails
    {
        public string ServiceBlockHash { get; set; }
        public double AmountPaid { get; set; }

        public InvoiceDetails(string serviceBlockHash, double amountPaid)
        {
            ServiceBlockHash = serviceBlockHash;
            AmountPaid = amountPaid;
        }
    }
}
