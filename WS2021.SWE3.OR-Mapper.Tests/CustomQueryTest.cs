using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WS2021.SWE3.EXAMPLE_APP.Model;

namespace WS2021.SWE3.OR_Mapper.Tests
{
    public class CustomQueryTest : BaseTest
    {
        [SetUp]
        public void Setup()
        {
            SetupRepositories();
            SetupTables();
        }

        /// <summary>
        /// Test Query for custom queries 
        /// Query in SQL: WHERE (author like '%ass' or price < 2) and id in ('5','6','7')
        /// </summary>
        [Test]
        public void QueryBooksTest()
        {
            Console.WriteLine($"Query book Test start");
            Book book1 = GenereateBook("5", price: 23, author: "Autass");
            Book book2 = GenereateBook("6", price: 1, author: "Tom");
            Book book3 = GenereateBook("7", price: 10, author: "GAs");
            Book book4 = GenereateBook("8", price: 10, author: "GAs");
            BookRepository.Save(book1);
            BookRepository.Save(book2);
            BookRepository.Save(book3);
            BookRepository.Save(book4);
            Console.WriteLine($"Total book list:");
            Console.WriteLine($"{book1.Id}, {book1.Price}, {book1.Author}");
            Console.WriteLine($"{book2.Id}, {book2.Price}, {book2.Author}");
            Console.WriteLine($"{book3.Id}, {book3.Price}, {book3.Author}");
            Console.WriteLine($"{book4.Id}, {book4.Price}, {book4.Author}");
            Console.WriteLine("Query in SQL: WHERE (author like '%ass' or price < 2) and id in ('5','6','7')");
            var queryGroup = BookRepository.CreateQuery().Like((book) => book.Author, "%ass").Or().LesserThan((book) => book.Price, 2).And().In((book) => book.Id, new() { "5", "6", "7" });
            var bookList = BookRepository.Query(queryGroup);
            CollectionAssert.Contains(bookList, book1);
            CollectionAssert.Contains(bookList, book2);
            CollectionAssert.DoesNotContain(bookList, book3);
            CollectionAssert.DoesNotContain(bookList, book4);
            Console.WriteLine($"Query book list:");
            foreach (var item in bookList)
            {
                Console.WriteLine($"{item.Id}, {item.Price}, {item.Author}");
            }
        }

        /// <summary>
        /// Test Query for custom queries 
        /// Query in SQL: WHERE (author like '%ass' or price < 2) and id in ('5','6','7')
        /// </summary>
        [Test]
        public void QueryEmployeesTest()
        {
            Console.WriteLine($"Query employee Test start");
            Employee employee1 = GenereateEmployee("53", firstname: "Autass");
            Employee employee2 = GenereateEmployee("55", firstname: "Autass");
            Employee employee3 = GenereateEmployee("54", firstname: "Autass");
            Employee employee4 = GenereateEmployee("56", firstname: "Autass");
            Library library1 = GenereateLibrary("122");
            employee2.WorkPlace = library1;
            library1.Employees.Add(employee2);
            EmployeeRepository.Save(employee1);
            EmployeeRepository.Save(employee2);
            EmployeeRepository.Save(employee3);
            EmployeeRepository.Save(employee4);
            LibraryRepository.Save(library1);
            Console.WriteLine($"Total employee list:");
            Console.WriteLine($"{employee1.Id}, {employee1.Firstname}");
            Console.WriteLine($"{employee2.Id}, {employee2.Firstname}");
            Console.WriteLine($"{employee3.Id}, {employee3.Firstname}");
            Console.WriteLine($"{employee4.Id}, {employee4.Firstname}");
            Console.WriteLine("Query in SQL: WHERE (author like '%ass' or price < 2) and id in ('5','6','7')");
            var queryGroup = EmployeeRepository.CreateQuery().Equals((emp) => emp.WorkPlace, library1);
            var employeeResultList = EmployeeRepository.Query(queryGroup);
            CollectionAssert.Contains(employeeResultList, employee2);
            CollectionAssert.DoesNotContain(employeeResultList, employee1);
            CollectionAssert.DoesNotContain(employeeResultList, employee3);
            CollectionAssert.DoesNotContain(employeeResultList, employee4);
            Console.WriteLine($"Query employee list:");
            foreach (var item in employeeResultList)
            {
                Console.WriteLine($"{item.Id}, {item.Firstname}, WorkPlace: {item.WorkPlace.Name}");
            }
        }
    }
}
