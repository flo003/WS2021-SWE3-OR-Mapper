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
                Id = 1,
                Firstname = "GES",
                BroughtBooks = new List<Book>()
               {
                   new Book()
                   {
                       Id = 1,
                       Name = "GOG"
                   }
                   
               }
            };
            dbSetup = new DbSetup();
            try
            {
                //dbSetup.WeatherForecastRepository.Setup();
                dbSetup.BookRepository.Setup();
                dbSetup.CustomerRepository.Setup();
            }
            catch(PostgresException exception)
            {
                Console.WriteLine(exception);
            }
            dbSetup.CustomerRepository.Save(customer);
            var customer1 = dbSetup.CustomerRepository.Get(1);
            Console.WriteLine(customer1);
            customer1.Firstname = "Test";
            customer1.BroughtBooks.First().ReleaseDate = DateTime.Now;
            dbSetup.CustomerRepository.Save(customer1);
            var customer2 = dbSetup.CustomerRepository.Get(1);
            Console.WriteLine(customer2);
            var book1 = dbSetup.BookRepository.Get(1);
            Console.WriteLine(book1);
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
