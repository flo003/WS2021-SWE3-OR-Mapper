using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    public class Employee : Person
    {
        public string EmployeeId { get; set; }
        public int Rating { get; set; }
    }
}
