using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class BusinessModel
    {
    

      public string? BusinessName {  get; set; }

        public int? vendorid{ get; set; }
        public string? ProfessionId { get; set; }
        public string? profession { get; set;}

        public string? Email { get; set;}
        public string? FaxNumber { get; set;}
        public string? PhoneNum { get;set;}

        public string? BusinessContact { get; set;}
    }
}
