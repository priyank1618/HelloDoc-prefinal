﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class Access
    {
        public string Name { get; set; }
        public short Accounttype { get; set; }
        public int roleid { get; set; }
        public List<int> Menu { get; set; }
    }
}
