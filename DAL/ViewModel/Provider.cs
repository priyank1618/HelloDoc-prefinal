using DAL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModel
{
    public class Provider
    {
        public List<Region>? regions { get; set; }

        public string Name { get; set; } = null!;

        public string Role { get; set; } = null!;

        public BitArray? OnCallStaus { get; set; }

        public short? status { get; set; }

        public int physicianid { get; set; }
    }
}
