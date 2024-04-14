using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ViewDoc
    {
        public List<RequestWiseFile> requestwisefile {  get; set; }

        public int? requestid { get; set;}

        public string? confirmationnum {  get; set; }
    }
}
