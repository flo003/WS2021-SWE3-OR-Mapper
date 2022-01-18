using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WS2021.SWE3.EXAMPLE_APP.Model;

namespace WS2021.SWE3.OR_Mapper.Tests
{
    public class OnetoNCrudTest : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            SetupRepositories();
            SetupTables();
        }

        /// <summary>
        /// Tests saving a employee with a library (1:n) and brought books (1:n).
        /// </summary>
        [Test]
        public void SaveEmployeeTest()
        {
            Console.WriteLine($"Save employee Test start");
            Employee employee1 = GenereateEmployee("1", firstname: "Tomwss");
            Library library1 = GenereateLibrary("1", address: "Gostrasse");
            employee1.WorkPlace = library1;
            library1.Employees.Add(employee1);
            EmployeeRepository.Save(employee1);
            Console.WriteLine($"Employee saved with id {employee1.Id} and firstname {employee1.Firstname} and workplace address {library1.Address}");
            var resultEmployee = EmployeeRepository.Get("1");
            Assert.AreEqual(employee1.Firstname, resultEmployee.Firstname);
            Assert.AreEqual(employee1.WorkPlace.Address, resultEmployee.WorkPlace.Address);
            Console.WriteLine($"Get Employee from Db with id {resultEmployee.Id} and firstname {resultEmployee.Firstname}");
            var resultLibrary = LibraryRepository.Get("1");
            Assert.AreEqual(library1.Address, resultLibrary.Address);
            Assert.AreEqual(library1.Employees.Single().Firstname, resultLibrary.Employees.Single().Firstname);
            Console.WriteLine($"Get Library from Db with id {resultLibrary.Id} and address {resultLibrary.Address} and first employee: {resultLibrary.Employees.Single().Firstname}");
        }

        /// <summary>
        /// Tests updating a employee with a library (1:n) and brought books (1:n).
        /// </summary>
        [Test]
        public void UpdateEmployeeTest()
        {
            Console.WriteLine($"Update employee Test start");
            Employee employee1 = GenereateEmployee("4", firstname: "Hes");
            Library library1 = GenereateLibrary("4", address: "Zorn");
            employee1.WorkPlace = library1;
            library1.Employees.Add(employee1);
            EmployeeRepository.Save(employee1);
            Console.WriteLine($"Employee saved with id {employee1.Id} and firstname {employee1.Firstname} and workplace address {library1.Address}");

            var resultEmployee = EmployeeRepository.Get("4");
            Assert.AreEqual(employee1.Firstname, resultEmployee.Firstname);
            Assert.AreEqual(employee1.WorkPlace.Address, resultEmployee.WorkPlace.Address);

            var resultLibrary = LibraryRepository.Get("4");
            Assert.AreEqual(library1.Address, resultLibrary.Address);
            Assert.AreEqual(library1.Employees.Single().Firstname, resultLibrary.Employees.Single().Firstname);
            Console.WriteLine($"Get Employee from Db with id {resultEmployee.Id} and firstname {resultEmployee.Firstname}");
            Console.WriteLine($"Get Library from Db with id {resultLibrary.Id} and address {resultLibrary.Address} and first employee: {resultLibrary.Employees.Single().Firstname}");
            
            resultEmployee.Firstname = "Op";
            resultEmployee.WorkPlace.Address = "lop";
            EmployeeRepository.Save(resultEmployee);
            Console.WriteLine($"Changed employee firstname to {resultEmployee.Firstname}");
            Console.WriteLine($"Changed library workplace to {resultEmployee.WorkPlace.Address}");
            var resultEmployee2 = EmployeeRepository.Get("4");
            Assert.AreEqual(resultEmployee.Firstname, resultEmployee2.Firstname);
            Assert.AreEqual(resultEmployee.WorkPlace.Address, resultEmployee2.WorkPlace.Address);

            var resultLibrary2 = LibraryRepository.Get("4");
            Assert.AreEqual(resultLibrary.Address, resultLibrary2.Address);
            Assert.AreEqual(resultLibrary.Employees.Single().Firstname, resultLibrary2.Employees.Single().Firstname);
            Console.WriteLine($"Get Employee from Db with id {resultEmployee2.Id} and firstname {resultEmployee2.Firstname}");
            Console.WriteLine($"Get Library from Db with id {resultLibrary2.Id} and address {resultLibrary2.Address} and first employee: {resultLibrary2.Employees.Single().Firstname}");
        }

        /// <summary>
        /// Tests deleting a employee with a library (1:n) and brought books (1:n).
        /// </summary>
        [Test]
        public void DeleteEmployeeTest()
        {
            Console.WriteLine($"Delete employee Test start");
            Employee employee1 = GenereateEmployee("8", firstname: "Forn");
            Library library1 = GenereateLibrary("8", address: "Testing");
            employee1.WorkPlace = library1;
            library1.Employees.Add(employee1);
            Console.WriteLine($"Employee saved with id {employee1.Id} and firstname {employee1.Firstname} and workplace address {library1.Address}");

            EmployeeRepository.Save(employee1);
            
            var resultEmployee = EmployeeRepository.Get("8");
            Assert.AreEqual(employee1.Firstname, resultEmployee.Firstname);
            Assert.AreEqual(employee1.WorkPlace.Address, resultEmployee.WorkPlace.Address);

            var resultLibrary = LibraryRepository.Get("8");
            Assert.AreEqual(library1.Address, resultLibrary.Address);
            Assert.AreEqual(library1.Employees.Single().Firstname, resultLibrary.Employees.Single().Firstname);
            Console.WriteLine($"Get Employee from Db with id {resultEmployee.Id} and firstname {resultEmployee.Firstname}");
            Console.WriteLine($"Get Library from Db with id {resultLibrary.Id} and address {resultLibrary.Address} and first employee: {resultLibrary.Employees.Single().Firstname}");

            EmployeeRepository.Delete(resultEmployee);
            Console.WriteLine($"Delete employee with id 8");
            var resultEmployeeNotFound = EmployeeRepository.Get("8");
            Console.WriteLine($"Get Employee from Db with id 8: {resultEmployeeNotFound}");
           Assert.IsNull(resultEmployeeNotFound);

            Library resultLibraryNoEmployee = LibraryRepository.Get("8");
            Console.WriteLine($"Get Library from Db with id {resultLibraryNoEmployee.Id} and address {resultLibraryNoEmployee.Address} and employee count: {resultLibraryNoEmployee.Employees.Count}");

        }
    }
}
