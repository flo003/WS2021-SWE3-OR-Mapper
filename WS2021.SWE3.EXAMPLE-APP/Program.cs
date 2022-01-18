using System;
using WS2021.SWE3.OR_Mapper.Tests;

namespace WS2021.SWE3.EXAMPLE_APP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("OR Mapper Tests");
            Console.WriteLine("----------------------------------------\n\n");
            BasicCrudTest basicCrudTest = new BasicCrudTest();
            basicCrudTest.SetupRepositories();
            basicCrudTest.SetupTables();
            OnetoNCrudTest onetoNCrudTest = new OnetoNCrudTest();
            onetoNCrudTest.SetupRepositories();
            onetoNCrudTest.SetupTables();
            NtoMCrudTest ntoMCrudTest = new NtoMCrudTest();
            ntoMCrudTest.SetupRepositories();
            ntoMCrudTest.SetupTables();
            CustomQueryTest customQueryTest = new CustomQueryTest();
            customQueryTest.SetupRepositories();
            customQueryTest.SetupTables();

            Console.WriteLine("Basix CRUD Tests with books and customers");
            Console.WriteLine("----------------------------------------");
            basicCrudTest.SaveBookTest();
            Console.WriteLine("");
            basicCrudTest.UpdateCustomerTest();
            Console.WriteLine("");
            basicCrudTest.DeleteCustomerTest();
            Console.WriteLine("\n");
            Console.WriteLine("1 to N CRUD Tests with employees");
            Console.WriteLine("----------------------------------------");
            onetoNCrudTest.SaveEmployeeTest();
            Console.WriteLine("");
            onetoNCrudTest.UpdateEmployeeTest();
            Console.WriteLine("");
            onetoNCrudTest.DeleteEmployeeTest();
            Console.WriteLine("\n");
            Console.WriteLine("N to M CRUD Tests with customers");
            Console.WriteLine("----------------------------------------");
            ntoMCrudTest.SaveCustomerTest();
            Console.WriteLine("");
            ntoMCrudTest.UpdateCustomerTest();
            Console.WriteLine("");
            ntoMCrudTest.DeletingCustomerTest();
            Console.WriteLine("\n");
            Console.WriteLine("Customer Query Tests");
            Console.WriteLine("----------------------------------------");
            customQueryTest.QueryEmployeesTest();
            Console.WriteLine("");
            customQueryTest.QueryBooksTest();
            Console.WriteLine("\n");
        }
    }
}
