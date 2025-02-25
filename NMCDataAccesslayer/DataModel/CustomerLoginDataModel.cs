using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMCDataAccesslayer.DataModel
{
   public class CustomerLoginDataModel
    {
        public CustomerLoginDataModel()
        {
            GoDown = new List<GodownDataModel>();
            Company = new List<CompanyDataModel>();
        }
        public long Id { get; set; }
        public string EmailId { get; set; }
        public string Phone { get; set; }
        public string CustomerId { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Deleted { get; set; }
        public string OTP { get; set; }
        public string GodownId { get; set; }
        public List<GodownDataModel> GoDown { get; set; }
        public bool isemail { get; set; }
        public string CompanyId { get; set; }
        public List<CompanyDataModel> Company { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
