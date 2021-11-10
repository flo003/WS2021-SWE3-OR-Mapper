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
        public string EmployeeId { get; set; }
        public int Rating { get; set; }
        [ForeignKey]
        public List<Library> WorkPlaces { get; set; } = new List<Library>();
    }
}
