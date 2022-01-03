using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WS2021.SWE3.EXAMPLE_APP.Model;

namespace WS2021.SWE3.OR_Mapper.Tests
{
    public class NtoMCrudTest : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            SetupRepositories();
            SetupTables();
        }

        /// <summary>
        /// Tests saving a customer with borrowed books (n:m) and brought books (n:m).
        /// </summary>
        [Test]
        public void SaveCustomerTest()
        {
            Customer customerExpect = GenereateCustomer("1");
            var borrowedBook = GenereateBook("1");
            customerExpect.BorrowedBooks.Add(borrowedBook);
            Book bookBrought = GenereateBook("2");
            customerExpect.BroughtBooks.Add(bookBrought);

            BookRepository.Save(borrowedBook);
            BookRepository.Save(bookBrought);
            CustomerRepository.Save(customerExpect);

            var customerResult = CustomerRepository.Get("1");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.BorrowedBooks.First(), customerResult.BorrowedBooks.First());
            Assert.AreEqual(customerExpect.BroughtBooks.First(), customerResult.BroughtBooks.First());

            var bookResult = BookRepository.Get("1");
            Assert.AreEqual(borrowedBook.Name, bookResult.Name);
        }

        /// <summary>
        /// Tests updating a customer with borrowed books (n:m) and brought books (n:m).
        /// </summary>
        [Test]
        public void UpdateCustomerTest()
        {
            Customer customerExpect = GenereateCustomer("91", firstname: "Frank2");
            var borrowedBook = GenereateBook("132", author: "Gon12");
            customerExpect.BorrowedBooks.Add(borrowedBook);
            Book bookBrought = GenereateBook("232", author: "Gon13");
            customerExpect.BroughtBooks.Add(bookBrought);

            BookRepository.Save(borrowedBook);
            BookRepository.Save(bookBrought);
            CustomerRepository.Save(customerExpect);

            var customerResult = CustomerRepository.Get("91");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.BorrowedBooks.First(), customerResult.BorrowedBooks.First());
            Assert.AreEqual(customerExpect.BroughtBooks.First(), customerResult.BroughtBooks.First());

            var bookResult = BookRepository.Get("132");
            Assert.AreEqual(borrowedBook.Name, bookResult.Name);

            customerResult.Firstname = "Tom2";

            bookResult.Author = "Goern3";
            BookRepository.Save(bookResult);
            CustomerRepository.Save(customerResult);

            var customerResult2 = CustomerRepository.Get("91");
            Assert.AreEqual(customerResult.Firstname, customerResult2.Firstname);
            Assert.AreEqual(customerResult.BorrowedBooks.First(), customerResult2.BorrowedBooks.First());
            Assert.AreEqual(customerResult.BroughtBooks.First(), customerResult2.BroughtBooks.First());

            var bookResult2 = BookRepository.Get("132");
            Assert.AreEqual(bookResult.Name, bookResult2.Name);
        }

        /// <summary>
        /// Tests deleting a customer with borrowed books (n:m) and brought books (n:m).
        /// </summary>
        [Test]
        public void DeletingCustomerTest()
        {
            Customer customerExpect = GenereateCustomer("1");
            var borrowedBook = GenereateBook("1");
            customerExpect.BorrowedBooks.Add(borrowedBook);
            Book bookBrought = GenereateBook("2");
            customerExpect.BroughtBooks.Add(bookBrought);

            BookRepository.Save(borrowedBook);
            BookRepository.Save(bookBrought);
            CustomerRepository.Save(customerExpect);

            var customerResult = CustomerRepository.Get("1");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.BorrowedBooks.First(), customerResult.BorrowedBooks.First());
            Assert.AreEqual(customerExpect.BroughtBooks.First(), customerResult.BroughtBooks.First());

            var bookResult = BookRepository.Get("1");
            Assert.AreEqual(borrowedBook.Name, bookResult.Name);

            CustomerRepository.Delete(customerResult);
            var customerResult2 = CustomerRepository.Get("1");
            Assert.IsNull(customerResult2);

        }
    }
}
