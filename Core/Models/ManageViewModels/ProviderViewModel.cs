using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.ManageViewModels
{
    public class ProviderViewModel
    {
        [Required]
        public string login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }

        [Required]
        public string imaphost { get; set; }

        public int imapport { get; set; }

        [Required]
        public string smtphost { get; set; }

        public int smtpport { get; set; }

        [Display(Name = "Use SSL")]
        public bool useSsl { get; set; }
        
        public ProviderViewModel()
        {
            imapport = 993;
            smtpport = 465;
        }

    }
}
