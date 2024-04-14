﻿using DAL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class ShiftDetailModel
    {
        public string? physicianName {  get; set; }

        public int? ShiftDetailId { get; set; }
        public string? Regioname { get; set; }

        public string? day { get; set;}

        public TimeOnly? starttime { get; set; }
        public TimeOnly? endtime { get; set; }

        public List<Region> Regions {  get; set; }

    }
}
