using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class Patient_login
    {
        [StringLength(256)]
        [Required]
        public string Email { get; set; }

        [Column(TypeName = "character varying")]
        [Required]
        public string PasswordHash { get; set; }
    }
}
