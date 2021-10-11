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
            Course course = new Course()
            {
               ID = "1",
               Name = "GES",
               Teacher = new Teacher()
               {
                   ID = "1",
                   FirstName = "Hello",
                   Name = "World",
                   
               }
            };
            dbSetup = new DbSetup();
            try
            {
                dbSetup.WeatherForecastRepository.Setup();
                dbSetup.CourseRepository.Setup();
            }
            catch(PostgresException exception)
            {
                Console.WriteLine(exception);
            }
            dbSetup.CourseRepository.Save(course);
            var course1 = dbSetup.CourseRepository.Get("1");
            Console.WriteLine(course1);
            course1.Name = "Test";
            dbSetup.CourseRepository.Save(course1);
            var course2 = dbSetup.CourseRepository.Get("1");
            Console.WriteLine(course2);
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
