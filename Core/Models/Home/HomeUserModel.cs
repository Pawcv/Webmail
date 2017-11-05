using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.Home
{
    public class HomeUserModel
    {
        [Required]
        [MaxLength(256)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Repeated password")]
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string RepeatedPassword { get; set; }
    }
}
