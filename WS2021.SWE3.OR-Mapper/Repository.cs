using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.CustomQuery;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper
{
    public class Repository<T>
    {
        private InternalRepository _internalRepository;
        public Repository(IDbConnection dbConnection, Dictionary<Type, string> createTablePropertiesConversion = null)
        {
            _internalRepository = new InternalRepository(typeof(T), dbConnection, createTablePropertiesConversion);
        }

        public Repository(IDbConnection dbConnection)
        {
            Dictionary<Type, string> createTablePropertiesConversion = new Dictionary<Type, string>()
            {
                [typeof(string)] = "varchar",
                [typeof(int)] = "numeric",
                [typeof(Int32)] = "numeric",
                [typeof(Int64)] = "numeric",
                [typeof(Int16)] = "numeric",
                [typeof(DateTime)] = "timestamp",
            };
            _internalRepository = new InternalRepository(typeof(T), dbConnection, createTablePropertiesConversion);
        }

        public void Delete(T value)
        {
            _internalRepository.Delete(value);
        }

        public void Save(T value)
        {
            _internalRepository.Save(value);
        }

        public T Get(object primaryKey)
        {
            return (T)_internalRepository.Get(primaryKey);
        }

        public void SetupTable()
        {
            _internalRepository.SetupTable();
        }
        public void SetupForeignKeys()
        {
            _internalRepository.SetupForeignKeys();
        }

        public QueryAction CreateQuery()
        {
            return _internalRepository.CreateQuery();
        }

        public List<T> Query(QueryGroup queryGroup)
        {
            return _internalRepository.Query<T>(queryGroup);
        }
    }
}
