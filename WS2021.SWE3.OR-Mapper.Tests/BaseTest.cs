using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.EXAMPLE_APP.Model;

namespace WS2021.SWE3.OR_Mapper.Tests
{
    public abstract class BaseTest
    {
        private Repository<Book> _bookRepository;
        private Repository<Customer> _customerRepository;
        private Repository<Library> _libraryRepository;
        private Repository<Employee> _employeeRepository;
        private string _connectionString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";
        private IDbConnection _databaseConnection;

        protected Repository<Book> BookRepository => _bookRepository;
        protected Repository<Library> LibraryRepository => _libraryRepository;
        protected Repository<Employee> EmployeeRepository => _employeeRepository;
        protected Repository<Customer> CustomerRepository => _customerRepository;

        public void SetupRepositories()
        {
            _databaseConnection = new NpgsqlConnection(_connectionString);
            _databaseConnection.Open();
            _bookRepository = new(_databaseConnection);
            _customerRepository = new(_databaseConnection);
            _libraryRepository = new(_databaseConnection);
            _employeeRepository = new(_databaseConnection);
        }

        public void SetupTables()
        {
            try
            {
                _bookRepository.SetupTable();
                _customerRepository.SetupTable();
                _libraryRepository.SetupTable();
                _employeeRepository.SetupTable();
                _bookRepository.SetupForeignKeys();
                _customerRepository.SetupForeignKeys();
                _libraryRepository.SetupForeignKeys();
                _employeeRepository.SetupForeignKeys();
            }
            catch (PostgresException exception)
            {
                 Console.WriteLine($"Setup Tables exception: {exception}");
            }
        }

        public Book GenereateBook(
            string id,
            string name = "Buch",
            string author = "Testo",
            string iban = "123123d",
            int price = 20,
            DateTime releaseDate = new DateTime()
            )
        {
            return new Book()
            {
                Id = id,
                Name = name,
                Author = author,
                IBan = iban,
                Price = price,
                ReleaseDate = releaseDate,
            };
        }

        public Customer GenereateCustomer(
            string id,
            string firstname = "Gustav",
            string lastname = "Ganz",
            string address = "Base Street 12",
            DateTime birthDate = new DateTime()
            )
        {
            return new Customer()
            {
                Id = id,
                Firstname = firstname,
                Lastname = lastname,
                Address = address,
                BorrowedBooks = new List<Book>(),
                BirthDate = birthDate,
                BroughtBooks = new List<Book>()
            };
        }

        public Employee GenereateEmployee(
            string id,
            string firstname = "Gustav",
            string lastname = "Ganz",
            string address = "Base Street 12",
            DateTime birthDate = new DateTime(),
            int rating = 2
            )
        {
            return new Employee()
            {
                Id = id,
                Firstname = firstname,
                Lastname = lastname,
                Address = address,
                BirthDate = birthDate,
                WorkPlace = null,
                Rating = rating,
            };
        }

        public Library GenereateLibrary(
            string id,
            string name = "Gustav Library",
            string address = "Base Street 12"
            )
        {
            return new Library()
            {
                Id = id,
                Name = name,
                Address = address,
                Employees = new List<Employee>(),
                StoredBooks = new List<Book>()
            };
        }
    }
}
