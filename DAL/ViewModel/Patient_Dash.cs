using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class Patient_Dash
    {
        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedDate { get; set; }

    
        public int CurrentStatus { get; set; }

        [StringLength(50)]
        public string? FilePath { get; set; }

        public int requestid {  get; set; }

        public int count { get; set; }

    }
}
