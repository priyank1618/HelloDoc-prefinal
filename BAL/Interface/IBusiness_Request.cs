using DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interface
{
    public interface IBusiness_Request
    {
        public void addbusinessdata(Other_Request Req);
    }
}
