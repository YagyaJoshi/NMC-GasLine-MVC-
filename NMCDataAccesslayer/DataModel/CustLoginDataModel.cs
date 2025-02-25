using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
    public class CustLoginDataModel
    {
        public string Id { get; set; }
        public string UniqueId { get; set; } 
        public string OTP { get; set; } 
        public string EmailId { get; set; } 
        public string Phone { get; set; }
        public bool isOTP { get; set; }
        public string Name { get; set; }
        public string CustomerId { get; set; }

        public string CustomerNumber { get; set; }
        public string password { get; set; }
    }
}
