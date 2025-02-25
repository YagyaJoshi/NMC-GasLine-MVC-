using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class User : IValidatableObject
    {
        public string Id { get; set; }

        public string UserId { get; set; }
        [StringLength(50, ErrorMessage = "Name must be between 1 and 50 char")]
        [Required(ErrorMessage = "Please Enter  Name")]
        public string Name { get; set; }

        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string Phone { get; set; }
        [StringLength(100, ErrorMessage = "Name must be between 1 and 100 char")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please Select Country Name")]
        public string CountryId { get; set; }
        public List<CountryMaster> Country { get; set; }
        [Required(ErrorMessage = "Please Select State Name")]
        public string StateId { get; set; }
        public List<StateMaster> State { get; set; }
        [Required(ErrorMessage = "Please Select City Name")]
        public string CityId { get; set; }
        public List<City> City { get; set; }

        [Required(ErrorMessage = "Please Select Company Name")]
        public string CompanyId { get; set; }
        public List<CompanyMaster> Company { get; set; }
        [Required(ErrorMessage = "Please Select Area Name")]
        public string AreaId { get; set; }
        public List<AreaMaster> Area { get; set; }
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Please Enter Valid Pincode")]
        public string Pincode { get; set; }

        public DateTime CreatedDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime ModifiedDate { get; set; }

        public int isActive { get; set; }

        [Required(ErrorMessage = "Please Enter Email ")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter valid email address")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "Please Enter Password")]
        //[DataType(DataType.Password)]
        public string Password { get; set; }
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string UpdatePassword { get; set; }
        public string CompanyName { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string AreaName { get; set; }

        public List<User> UserList { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public string RoleName { get; set; }

        public string GoDownId { get; set; }
        public List<GodownMaster> GoDown { get; set; }
        public List<StockItemMaster> StockItemList { get; set; }

        public Int32 TotalRows { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(Password))
                yield return new ValidationResult("Please Enter Password.");
        }
        [Required(ErrorMessage = "Please Enter Role")]
        public string RoleId { get; set; }
        public List<Role> Role { get; set; }
        public string oldRoleName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string BillFileName { get; set; }
        public string PaymentFileName { get; set; }
        public string filedate { get; set; }
        public string EmpCode { get; set; }
       
    }
}