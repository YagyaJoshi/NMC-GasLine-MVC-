using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NMCPipedGasLineAPI.Models
{
    public class CustLogin : IValidatableObject
    {
       
        public string Id { get; set; }
        //in otp
       // [Required(ErrorMessage = "Please enter Customer Id")]
        //[Display(Name = "UniqueId")]
        public string UniqueId { get; set; }

        [Display(Name = "OTP")]
        public string OTP { get; set; }

        [Display(Name = "Email")]
        public string EmailId { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        public bool isOTP { get; set; }

        public string Name { get; set; }
       
        public string CustomerId { get; set; }

        [Required]
        public string CustomerNumber { get; set; }
       // [Required(ErrorMessage = "Please enter Password")]
       [Required]
        public string password { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {  //in otp
            //if (string.IsNullOrEmpty(EmailId) && string.IsNullOrEmpty(Phone))
            //{
            //    yield return new ValidationResult("Please enter any one Email or Phone");
            //}
            if (string.IsNullOrEmpty(CustomerNumber))
            {
                yield return new ValidationResult("Please enter Customer Id");
            }
            if (string.IsNullOrEmpty(password))
            {
                yield return new ValidationResult("Please enter Password");
            }
        }

    }

}