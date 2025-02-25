using NMCDataAccesslayer.DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class CustomerLogin : IValidatableObject
    {
        public CustomerLogin()
        {
            GoDown = new List<GodownDataModel>();
            Company = new List<CompanyDataModel>();
        }
        public long Id { get; set; }

        //[Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string EmailId { get; set; }

        //[Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "CustomerId")]
        public string CustomerId { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Deleted { get; set; }
        public string OTP { get; set; }

        [Required(ErrorMessage = "Please Select Society")]
        public string GodownId { get; set; }
        public List<GodownDataModel> GoDown { get; set; }

        [Required(ErrorMessage = "Please Select Company")]
        public string CompanyId { get; set; }
        public List<CompanyDataModel> Company { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool isemail { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(EmailId) && string.IsNullOrEmpty(Phone))
            {
                yield return new ValidationResult("Please enter any one EmailId or Phone");
            }
        }
    }
    public class ForgotPwd
    {

        [Required(ErrorMessage = "Customer Id is required.")]

        public string EmailId { get; set; }

        public string Password { get; set; }
    }
    public class ChangePwd
    {
        public string EmailId { get; set; }
        public string Id { get; set; }
       
        
        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(20, ErrorMessage = "The {0} should not be more than 20 characters")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation Password is required.")]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and Confirmation Password must match.")]
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
    }

}