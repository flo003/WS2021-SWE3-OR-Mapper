using System;
using WS2021.SWE3.OR_Mapper.Tests;

namespace WS2021.SWE3.EXAMPLE_APP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("OR Mapper Tests");
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
            basicCrudTest.SaveBookTest();
            basicCrudTest.UpdateCustomerTest();
            basicCrudTest.DeleteCustomerTest();

            Console.WriteLine("1 to N CRUD Tests with employees");
            onetoNCrudTest.SaveEmployeeTest();
            onetoNCrudTest.UpdateEmployeeTest();
            onetoNCrudTest.DeleteEmployeeTest();

            Console.WriteLine("N to M CRUD Tests with customers");
            ntoMCrudTest.SaveCustomerTest();
            ntoMCrudTest.UpdateCustomerTest();
            ntoMCrudTest.DeletingCustomerTest();

            Console.WriteLine("Customer Query Tests");
            customQueryTest.QueryEmployeesTest();
            customQueryTest.QueryBooksTest();
        }
    }
}
