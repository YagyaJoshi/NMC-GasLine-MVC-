using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
    public class ChangPwDataModel
    {
        public string EmailId { get; set; }
        public string Id { get; set; }

        public string Password { get; set; }

        public string Name { get; set; }
        public string ConfirmPassword { get; set; }
    }


    public class CustomerEmailSMS
    {
        public string Email { get; set; }
        public string Id { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string CustomerNumber { get; set; }
        public string Type { get; set; }
        public string EmailMsg { get; set; }
        public string SMSMsg { get; set; }
    }
}
