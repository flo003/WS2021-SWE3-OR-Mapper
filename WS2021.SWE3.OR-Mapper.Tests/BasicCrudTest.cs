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
            Book bookExpect = GenereateBook("3");
            BookRepository.Save(bookExpect);
            var bookResult = BookRepository.Get("3");
            Assert.AreEqual(bookExpect.Name, bookResult.Name);
            Assert.AreEqual(bookExpect.Price, bookResult.Price);
            Assert.AreEqual(bookExpect.ReleaseDate, bookResult.ReleaseDate);
        }

        /// <summary>
        /// Delete a customer entity and getting null from the db
        /// </summary>
        [Test]
        public void DeleteCustomerTest()
        {
            Customer customerExpect = GenereateCustomer("8");
            CustomerRepository.Save(customerExpect);
            var customerResult = CustomerRepository.Get("8");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.Address, customerResult.Address);
            CustomerRepository.Delete(customerResult);
            var customerResult2 = CustomerRepository.Get("8");
            Assert.IsNull(customerResult2);
        }

        /// <summary>
        /// Updating a customer entity and getting it back from the db
        /// </summary>
        [Test]
        public void UpdateCustomerTest()
        {
            Customer customerExpect = GenereateCustomer("10", firstname: "Go");
            CustomerRepository.Save(customerExpect);
            var customerResult = CustomerRepository.Get("10");
            Assert.AreEqual(customerExpect.Firstname, customerResult.Firstname);
            Assert.AreEqual(customerExpect.Address, customerResult.Address);

            customerResult.Firstname = "GG";

            CustomerRepository.Save(customerResult);
            var customerResult2 = CustomerRepository.Get("10");
            Assert.AreEqual(customerResult.Firstname, customerResult2.Firstname);
            Assert.AreEqual(customerResult.Address, customerResult2.Address);
            Assert.AreEqual(customerExpect.Address, customerResult2.Address);
        }





        
    }
}