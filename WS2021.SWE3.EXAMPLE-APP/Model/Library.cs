using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelAttributes;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    [Entity]
    public class Library
    {
        public string Name { get; set; }
        public string Address { get; set; }
        [ForeignKey]
        public List<Book> StoredBooks { get; set; } = new List<Book>();
        [ForeignKey]
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
