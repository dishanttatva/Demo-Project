using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerEntity.ViewModel
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Passowrd is Required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "The password must have at least one lowercase, one uppercase, one numeric and one special character.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "FirstName is Required")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "LastName is Required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "DateOfBirth is Required")]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "PhoneNumber is Required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "PhoneNumber should be of length 10")]
        //[RegularExpression(@"^\d{10}$", ErrorMessage = "Please Enter valid PhoneNumber")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage ="Repeat Password is required")]
        [Compare(nameof(Password),ErrorMessage ="Password and cofirm password does not match")]
        public string RePassword { get; set; } = null!;
    }
}
