﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myJIRA.Models
{
    public class BoardName
    {   
        public int? ID { get; private set; }
        public string Name { get; set; }
        public int Order { get; set; }

        public BoardName(string name, int order)
        {
            Name = name;
            Order = order;
        }

        //DB Load
        public BoardName(int id, string name, int order) : this(name, order)
        {
            ID = id;
        }

        public BoardName(string name)
        {
            Name = name;
        }
    }
}
