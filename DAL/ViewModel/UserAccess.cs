﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class UserAccess
    {
        public string AccountPOC { get; set; }
        public string AccountType { get; set; }
        public int AccountTypeid { get; set; }

        public string phonenum { get; set; }
        public int roleid { get; set; }

        public int? useraccessid { get; set; }
        public int? status { get; set; }
        public int? OpenRequest { get; set; }
    }
}
