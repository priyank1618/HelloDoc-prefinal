using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class SendOrder
    { 
        public List<HealthProfessionalType>? ProfessionName { get; set; }
        public string? Name { get; set; }
        public int? requestid { get; set; }

        public int? vendorId { get; set; }

       public string? BusinessContact { get; set; }

        public string? Email { get; set; }

        public string? FaxNum{ get; set; }

        public string? Disciription { get; set; }

        public List<string>? Retail { get; set;}

    }
}
