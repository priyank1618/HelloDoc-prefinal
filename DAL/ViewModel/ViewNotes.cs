using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ViewNotes
    {


        public String? AdminNotes { get; set; }

        public String? TransferNotes { get; set; }

        public String? PhysicianNotes { get; set; }

        public String AdditionalNotes { get; set; }

        public int RequestId { get; set; }
    }
}
