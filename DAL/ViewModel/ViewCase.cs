using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ViewCase
    {
        public int? RequestClientId { get; set; }  

        public string? ConfirmationNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Notes { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public string address { get; set; }

        public string? region { get; set; }

        public int? requestid { get; set; }

        public int? status { get; set; }

        public List<CaseTag> cases { get; set; }

        //public string Location { get; set; }
    }
}
