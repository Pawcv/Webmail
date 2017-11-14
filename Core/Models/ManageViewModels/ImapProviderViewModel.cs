using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.ManageViewModels
{
    public class ImapProviderViewModel
    {
        [Required]
        public string login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        public string host { get; set; }

        public int port { get; set; }

        [Display(Name = "Use SSL")]
        public bool useSsl { get; set; }
    }
}
