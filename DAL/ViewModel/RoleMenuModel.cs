using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class RoleMenuModel
    {
        public List<Menu> MenuList { get; set; }
        public List<int> RoleMenuIds { get; set; }
    }
}
