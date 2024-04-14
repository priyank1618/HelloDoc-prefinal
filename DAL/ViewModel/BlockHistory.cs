﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class BlockHistory
    {
		public int BlockedRequestID { get; set; }

		public int RequestId { get; set; }

		public string PatientName { get; set; }

		public DateTime? CreatedDate { get; set; }

		public string PhoneNumber { get; set; }
		public string Email { get; set; }

		public string Notes { get; set; }

		public bool? IsActive { get; set; }
	}
}
