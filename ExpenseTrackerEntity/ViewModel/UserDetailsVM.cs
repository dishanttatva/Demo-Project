using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerEntity.ViewModel
{
    public class UserDetailsVM
    {




        [Required(ErrorMessage ="Email is Required")]

        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "FirstName is Required")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "LastName is Required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "DateOfBirth is Required")]
        public DateOnly DateOfBirth { get; set; }
        [Required(ErrorMessage = "PhoneNumber is Required")]
        [MaxLength(10,ErrorMessage ="Phone number should be length of 10")]
        public string? PhoneNumber { get; set; }

        public string? Password { get; set; }
    }
}
