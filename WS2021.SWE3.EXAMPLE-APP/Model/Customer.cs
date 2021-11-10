using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelAttributes;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    [Entity]
    public class Customer : Person
    {
        [ForeignKey]
        public List<Book> BorrowedBooks { get; set; } = new List<Book>();
        [ForeignKey(RemoteTableName = "BookOwnedByCustomer", RemoteTableColumnName = "broughtbookid", ColumnName = "customerid")]
        public List<Book> BroughtBooks { get; set; } = new List<Book>();

    }
}
