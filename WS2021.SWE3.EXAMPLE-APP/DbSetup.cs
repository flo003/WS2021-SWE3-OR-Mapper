using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WS2021.SWE3.EXAMPLE_APP.Model;
using WS2021.SWE3.OR_Mapper;

namespace WS2021.SWE3.EXAMPLE_APP
{
    public class DbSetup
    {
        private NpgsqlConnection _databaseConnection;

        private string connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";

        private Repository<Book> bookRepository;
        private Repository<Customer> customerRepository;
        public Repository<Book> BookRepository { get { return bookRepository; } }
        public Repository<Customer> CustomerRepository { get { return customerRepository; } }
        public DbSetup()
        {
            ConnectDatabase();
        }

        public void ConnectDatabase()
        {
            try
            {
                _databaseConnection = new NpgsqlConnection(connectionString);
                _databaseConnection.Open();
                Dictionary<Type, string> createTablePropertiesConversion = new Dictionary<Type, string>() {
                    [typeof(string)] = "varchar",
                    [typeof(int)] = "numeric",
                    [typeof(Int32)] = "numeric",
                    [typeof(Int64)] = "numeric",
                    [typeof(Int16)] = "numeric",
                    [typeof(DateTime)] = "timestamp",
                };
                bookRepository = new Repository<Book>(_databaseConnection, createTablePropertiesConversion);
                customerRepository = new Repository<Customer>(_databaseConnection, createTablePropertiesConversion);
            }
            catch (PostgresException exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
