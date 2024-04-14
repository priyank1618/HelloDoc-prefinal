using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class GetCount
    {
        public int NewCount { get; set; }
        public int PendingCount { get; set; }
        public int ActiveCount { get; set; }
        public int Conclude { get; set; }
        public int ToClosed { get; set; }
        public int Unpaid { get; set; }
    }
}
