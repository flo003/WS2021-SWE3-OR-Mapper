﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelAttributes;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    public class Person
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime BirthDate { get; set; }
        public string Address { get; set; }
    }
}
