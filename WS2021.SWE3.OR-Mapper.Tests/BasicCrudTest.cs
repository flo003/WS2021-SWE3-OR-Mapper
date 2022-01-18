using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WS2021.SWE3.EXAMPLE_APP.Model;

namespace WS2021.SWE3.OR_Mapper.Tests
{
    public class BasicCrudTest : BaseTest
    {

        [SetUp]
        public void Setup()
        {
            SetupRepositories();
            SetupTables();
        }

        /// <summary>
        /// Saving a book entity and getting it back from the db
        /// </summary>
        [Test]
        public void SaveBookTest()
        {
            Console.WriteLine($"Save Book Test start");
            Book bookExpect = GenereateBook("3", price: 42);
            Console.WriteLine($"Save Book with id {bookExpect.Id} and price {bookExpect.Price}");
            BookRepository.Save(bookExpect);
            var bookResult = BookRepository.Get("3");
            Assert.AreEqual(bookExpect.Name, bookResult.Name);
            Assert.AreEqual(bookExpect.Price, bookResult.Price);
            Assert.AreEqual(bookExpect.ReleaseDate, bookResult.ReleaseDate);
            Console.WriteLine($"Get Book from Db with id {bookResult.Id}, it has price {bookResult.Price}");
        }

        /// <summary>
        /// Delete a customer entity and getting null from the db
        /// </summary>
        [Test]
        public void DeleteCustomerTest()
        {
            Console.WriteLine($"Delete Customer start");
            Customer customerExpect = GenereateCustomer("8", firstname: "FFS", address: "Herns");
            CustomerRepository.Save(customerExpect);
            var customerResult = CustomerRepository.Get("8");
            Console.WriteLine($"Get Customer from Db with id {customerResult.Id} and has firstname {customerResult.Firstname}");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.Address, customerResult.Address);
            CustomerRepository.Delete(customerResult);
            Console.WriteLine($"Delete Customer from Db with id {customerResult.Id}");
            var customerResult2 = CustomerRepository.Get("8");
            Console.WriteLine($"Get Customer from Db with id {customerResult.Id}: {customerResult2}");
            Assert.IsNull(customerResult2);
        }

        /// <summary>
        /// Updating a customer entity and getting it back from the db
        /// </summary>
        [Test]
        public void UpdateCustomerTest()
        {
            Console.WriteLine($"Update customer start");
            Customer customerExpect = GenereateCustomer("10", firstname: "Go", address: "Test");
            CustomerRepository.Save(customerExpect);
            var customerResult = CustomerRepository.Get("10");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.Address, customerResult.Address);
            Console.WriteLine($"Get Customer from Db with id {customerResult.Id}: {customerResult.Firstname}");
            customerResult.Firstname = "GG";

            CustomerRepository.Save(customerResult);
            Console.WriteLine($"Save Customer with id {customerResult.Id} and changed firstname {customerResult.Firstname}");
            var customerResult2 = CustomerRepository.Get("10");
            Assert.AreEqual(customerResult.Firstname, customerResult2.Firstname);
            Assert.AreEqual(customerResult.Address, customerResult2.Address);
            Assert.AreEqual(customerExpect.Address, customerResult2.Address);
            Console.WriteLine($"Get Customer from Db with id {customerResult2.Id} and firstname {customerResult2.Firstname}");
        }






    }
}