using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelAttributes;

namespace WS2021.SWE3.EXAMPLE_APP.Model
{
    [Entity]
    public class Book
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int Price { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string IBan { get; set; }
        [ForeignKey(RemoteTableName = "BookBorrowedByCustomer", RemoteTableColumnName = "customerid", ColumnName = "borrowedbookid")]
        public List<Customer> CustomerBorrowed { get; set; } = new List<Customer>();
        [ForeignKey(RemoteTableName = "BookOwnedByCustomer", RemoteTableColumnName = "customerid", ColumnName = "broughtbookid")]
        public List<Customer> OwnedBy { get; set; } = new List<Customer>();

        [ForeignKey(RemoteTableName = "BookInLibrary", RemoteTableColumnName = "libraryId", ColumnName = "rentbookid")]
        public List<Library> InLibrary { get; set; } = new List<Library>();
    }
}
