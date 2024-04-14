using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class CloseCase
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? ConfirmationNum { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public int? requestid { get; set; }
        public List<RequestWiseFile> Files { get; set; }

        public string? Email { get; set; }
        public string? Phonenum { get; set; }
    }
}
