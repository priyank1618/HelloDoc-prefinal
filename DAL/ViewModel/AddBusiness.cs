using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class AddBusiness
    {
        public string BusinessName { get; set; }
        public string Profession { get; set;}

        public int vendorid { get; set; }

        public string? FaxNumber { get; set; }
        public string? PhoneNum{ get; set; } 
        public string Email { get; set; }
        public string? BusinessContact { get; set; }
        public string? street { get; set; }
        public string? city { get; set;}

        public string state { get; set; } 
        public string? zipcode { get; set;}
    }

       
    }

