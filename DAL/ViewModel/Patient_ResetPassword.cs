﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class Patient_ResetPassword
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword {get; set;}

        public string? token { get; set; }
    }
}
