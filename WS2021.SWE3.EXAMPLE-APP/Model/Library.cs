using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    public class Library
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Book> StoredBooks { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
