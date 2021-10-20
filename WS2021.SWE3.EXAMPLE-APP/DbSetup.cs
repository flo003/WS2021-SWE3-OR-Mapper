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

        private Repository<WeatherForecast> weatherForecastRepository;
        private Repository<Course> courseRepository;
        public Repository<WeatherForecast> WeatherForecastRepository { get { return weatherForecastRepository; } }
        public Repository<Course> CourseRepository { get { return courseRepository; } }
        private Repository<Teacher> teacherRepository;
        public Repository<Teacher> TeacherRepository { get { return teacherRepository; } }
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
                weatherForecastRepository = new Repository<WeatherForecast>(_databaseConnection, createTablePropertiesConversion);
                weatherForecastRepository.Connection = _databaseConnection;
                courseRepository = new Repository<Course>(_databaseConnection, createTablePropertiesConversion);
                courseRepository.Connection = _databaseConnection;
                teacherRepository = new Repository<Teacher>(_databaseConnection, createTablePropertiesConversion);
                teacherRepository.Connection = _databaseConnection;
            }
            catch (PostgresException exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
