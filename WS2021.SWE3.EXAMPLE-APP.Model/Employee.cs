using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelAttributes;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    [Entity]
    public class Employee : Person
    {
        public int Rating { get; set; }
        [ForeignKey]
        public Library WorkPlace { get; set; } = null;
    }
}
