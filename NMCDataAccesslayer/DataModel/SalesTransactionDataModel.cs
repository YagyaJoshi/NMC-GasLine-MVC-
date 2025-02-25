using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
    class SalesTransactionDataModel
    {

        public string SaleId { get; set; }

        public int ReferenceNo { get; set; }

        public string CustomerId { get; set; }

        public string SalesLedger { get; set; }

        public DateTime SaleDate { get; set; }

        public int NewReading { get; set; }

        public int PreviousReading { get; set; }

        public DateTime ReadingDate { get; set; }

        public string Narration { get; set; }

        public DateTime CreatedDate { get; set; }

        public string XmlTally { get; set; }

        public int XmlSendToTally { get; set; }

        public int TallyResponseSuccess { get; set; }





    }
}
