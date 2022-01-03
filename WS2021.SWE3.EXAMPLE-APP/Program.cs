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


            basicCrudTest.SaveBookTest();
            basicCrudTest.UpdateCustomerTest();
            basicCrudTest.DeleteCustomerTest();

            onetoNCrudTest.SaveEmployeeTest();
            onetoNCrudTest.UpdateEmployeeTest();
            onetoNCrudTest.DeleteEmployeeTest();

            ntoMCrudTest.SaveCustomerTest();
            ntoMCrudTest.UpdateCustomerTest();
            ntoMCrudTest.DeletingCustomerTest();

            customQueryTest.QueryEmployeesTest();
            customQueryTest.QueryBooksTest();
        }
    }
}
