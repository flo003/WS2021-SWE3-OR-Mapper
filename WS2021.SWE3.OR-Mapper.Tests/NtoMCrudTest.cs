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
            Console.WriteLine($"Save employee Test start");
            Customer customerExpect = GenereateCustomer("1", firstname: "Gorn");
            var borrowedBook = GenereateBook("1", name: "Alfferd");
            customerExpect.BorrowedBooks.Add(borrowedBook);
            Book bookBrought = GenereateBook("2", name: "Lorn");
            customerExpect.BroughtBooks.Add(bookBrought);
            
            BookRepository.Save(borrowedBook);
            BookRepository.Save(bookBrought);
            CustomerRepository.Save(customerExpect);
            Console.WriteLine($"Customer saved with id {customerExpect.Id} and owned book {bookBrought.Name} and borrowed book {borrowedBook.Name}");

            var customerResult = CustomerRepository.Get("1");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.BorrowedBooks.First(), customerResult.BorrowedBooks.First());
            Assert.AreEqual(customerExpect.BroughtBooks.First(), customerResult.BroughtBooks.First());
            Console.WriteLine($"Get Customer from Db with id {customerResult.Id} and firstname {customerResult.Firstname}");

            var bookResult = BookRepository.Get("1");
            Assert.AreEqual(borrowedBook.Name, bookResult.Name);
            Console.WriteLine($"Get Book from Db with {bookResult.Id} and name {bookResult.Name}");
        }

        /// <summary>
        /// Tests updating a customer with borrowed books (n:m) and brought books (n:m).
        /// </summary>
        [Test]
        public void UpdateCustomerTest()
        {
            Console.WriteLine($"Update customer Test start");
            Customer customerExpect = GenereateCustomer("91", firstname: "Frank2");
            var borrowedBook = GenereateBook("132", author: "Gon12");
            customerExpect.BorrowedBooks.Add(borrowedBook);
            Book bookBrought = GenereateBook("232", author: "Gon13");
            customerExpect.BroughtBooks.Add(bookBrought);

            BookRepository.Save(borrowedBook);
            BookRepository.Save(bookBrought);
            CustomerRepository.Save(customerExpect);
            Console.WriteLine($"Customer saved with id {customerExpect.Id} and owned book {bookBrought.Name} and borrowed book {borrowedBook.Name}");

            var customerResult = CustomerRepository.Get("91");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.BorrowedBooks.First(), customerResult.BorrowedBooks.First());
            Assert.AreEqual(customerExpect.BroughtBooks.First(), customerResult.BroughtBooks.First());

            var bookResult = BookRepository.Get("132");
            Assert.AreEqual(borrowedBook.Name, bookResult.Name);
            Console.WriteLine($"Get Customer from Db with id {customerResult.Id} and borrowed book {bookResult.Name}");

            customerResult.Firstname = "Tom2";
            bookResult.Author = "Goern3";
            BookRepository.Save(bookResult);
            CustomerRepository.Save(customerResult);
            Console.WriteLine($"Customer with id {customerResult.Id}: firstname updated to {customerResult.Firstname}");
            Console.WriteLine($"Book with id {bookResult.Id}: author updated to {bookResult.Author}");

            var customerResult2 = CustomerRepository.Get("91");
            Assert.AreEqual(customerResult.Firstname, customerResult2.Firstname);
            Assert.AreEqual(customerResult.BorrowedBooks.First(), customerResult2.BorrowedBooks.First());
            Assert.AreEqual(customerResult.BroughtBooks.First(), customerResult2.BroughtBooks.First());
            Console.WriteLine($"Get Customer from Db with id {customerResult2.Id} and owned book {customerResult.BroughtBooks.First().Name} and borrowed book {customerResult.BorrowedBooks.First().Name}");

            var bookResult2 = BookRepository.Get("132");
            Assert.AreEqual(bookResult.Name, bookResult2.Name);
        }

        /// <summary>
        /// Tests deleting a customer with borrowed books (n:m) and brought books (n:m).
        /// </summary>
        [Test]
        public void DeletingCustomerTest()
        {
            Console.WriteLine($"Delete customer Test start");
            Customer customerExpect = GenereateCustomer("1");
            var borrowedBook = GenereateBook("1");
            customerExpect.BorrowedBooks.Add(borrowedBook);
            Book bookBrought = GenereateBook("2");
            customerExpect.BroughtBooks.Add(bookBrought);

            BookRepository.Save(borrowedBook);
            BookRepository.Save(bookBrought);
            CustomerRepository.Save(customerExpect);
            Console.WriteLine($"Customer saved with id {customerExpect.Id} and owned book {bookBrought.Name} and borrowed book {borrowedBook.Name}");

            var customerResult = CustomerRepository.Get("1");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.BorrowedBooks.First(), customerResult.BorrowedBooks.First());
            Assert.AreEqual(customerExpect.BroughtBooks.First(), customerResult.BroughtBooks.First());

            var bookResult = BookRepository.Get("1");
            Assert.AreEqual(borrowedBook.Name, bookResult.Name);
            Console.WriteLine($"Get Customer from Db with id {customerResult.Id} and borrowed book {bookResult.Name}");

            CustomerRepository.Delete(customerResult);
            Console.WriteLine($"Delete customer with id 1");

            var customerResult2 = CustomerRepository.Get("1");
            Assert.IsNull(customerResult2);
            Console.WriteLine($"Get customer from db with id 1: {customerResult2}");
        }
    }
}
