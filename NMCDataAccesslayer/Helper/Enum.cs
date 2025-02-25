using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.Helper
{
    class Enum
    {
        public enum PaymentMode
        {
            Cash = 1,
            Cheque = 2,
            online=3

        }


        public enum BillType
        {
            Deposit = 1,
            GasConsume = 2
        }
        
        public enum ExportStatus
        {
            New = 0,
            Exported = 1,
            Update = 2
        }

        public enum PaymentType
        {
            Partial = 1,
            Full = 2
        }


        public enum PaymentFrom
        {
            Mobile = 1,
            Web = 2
        }
        //in table IsActive
        public enum CustomerStatus
        {
            Connect = 1,
            Disconnected = 2
        }
    }
}
