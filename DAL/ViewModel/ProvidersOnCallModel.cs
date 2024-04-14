using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ProvidersOnCallModel

    {
        public List<Physician>? OnDuty { get; set; }
        public List<Physician>? OffDuty { get; set; }

    }
}
