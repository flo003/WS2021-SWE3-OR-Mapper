using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    public class Customer : Person
    {
        public List<Book> BorrowedBooks { get; set; }
        public List<Book> BroughtBooks { get; set; }

    }
}
