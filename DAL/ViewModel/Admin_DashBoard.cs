using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class Admin_DashBoard
    {
         [StringLength(100)]
        public string? Name { get; set; } 

         public string? LastName { get; set; }

        public int? physicianid { get; set; }

        public string? BirthDate { get; set;}

        public string? Email { get; set; }
       

        public string? Requestor { get; set; }

        public  DateTime? RequestedDate { get; set; }

        public string? PhoneNumber { get; set; }
        public string? PhoneNumber_P{ get; set; }

        public string? Address {  get; set; }

        public string? Notes { get; set; }

        public int? requesttypeid { get; set; }

        public int? regionid { get; set; }

        public string? physicianName {  get; set; }

        public string? dateOfService {  get; set; }

        public string regionname { get; set; }

        public int status { get; set;}

        public int? reqclientid { get; set;}
        public int? requestid { get; set;}

        public string? confirmationnum {  get; set; }

        public bool? Isfinalise { get; set; }

      public  List<CaseTag>? cases { get; set; }
      public  List<Region>? region { get; set; }

        
    }
}
