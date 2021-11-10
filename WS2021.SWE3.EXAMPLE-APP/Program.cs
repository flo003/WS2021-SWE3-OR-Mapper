using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS2021.SWE3.EXAMPLE_APP.Model;
using WS2021.SWE3.OR_Mapper;

namespace WS2021.SWE3.EXAMPLE_APP
{
    public class Program
    {
        private static DbSetup dbSetup;
        public static void Main(string[] args)
        {
            Customer customer = new Customer()
            {
                Id = "1",
                Firstname = "GES",
                Lastname = "Test",
                Address = "asd ",
                BorrowedBooks = new List<Book>()
               {
                   new Book()
                   {
                       Id = "1",
                       Name = "Buch 1",
                       Author = "Tes",
                       IBan = "123123d",
                       ReleaseDate = DateTime.Now,
                   }
                   
               }
            };
            Book bookBrought = new()
            {
                Id = "2",
                Name = "Book gekauft",
                Author = "2222",
                IBan = "123123d",
                ReleaseDate = DateTime.Now,
            };
            Book book3 = new()
            {
                Id = "3",
                Name = "Book 3",
                Author = "2222",
                IBan = "123123d",
                ReleaseDate = DateTime.Now,
            };
            customer.BorrowedBooks.First().BorrowedBooks = customer;
            customer.BroughtBooks.Add(bookBrought);
            dbSetup = new DbSetup();
            try
            {
                //dbSetup.WeatherForecastRepository.Setup();
                dbSetup.BookRepository.SetupTable();
                dbSetup.CustomerRepository.SetupTable();
                dbSetup.BookRepository.SetupForeignKeys();
                dbSetup.CustomerRepository.SetupForeignKeys();
            }
            catch(PostgresException exception)
            {
                Console.WriteLine(exception);
            }
            dbSetup.BookRepository.Save(bookBrought);
            dbSetup.BookRepository.Save(customer.BorrowedBooks.First());
            dbSetup.BookRepository.Save(book3);
            dbSetup.CustomerRepository.Save(customer);
            var customer1 = dbSetup.CustomerRepository.Get("1");
            Console.WriteLine($"{customer1.Id} {customer1.Firstname} {customer1.Lastname}");
            customer1.Firstname = "Test";
            customer1.BorrowedBooks.First().ReleaseDate = DateTime.Now;
            dbSetup.CustomerRepository.Save(customer1);
            var customer2 = dbSetup.CustomerRepository.Get("1");
            Console.WriteLine($"{customer2.Id} {customer2.Firstname} {customer2.Lastname}");
            var book1 = dbSetup.BookRepository.Get("2");
            Console.WriteLine($"{book1.Id} {book1.Name}");
            var book3Delete = dbSetup.BookRepository.Get("3");
            Console.WriteLine($"{book3Delete.Id} {book3Delete.Name}");
            dbSetup.BookRepository.Delete(book3Delete);
            dbSetup.BookRepository.Delete(book1);
            //  CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
