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
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">Type of Entity managed by the repository</typeparam>
    /// <typeparam name="P">Type of the primary key of the entity</typeparam>
    public class Repository<T, P>
    {
        private InternalRepository _internalRepository;
        /// <summary>
        /// Creates a Repository of type T with a custom conversion table for the database types to c# types conversion
        /// </summary>
        /// <param name="dbConnection">Connection to database</param>
        /// <param name="createTablePropertiesConversion">Conversion table for database types</param>
        public Repository(IDbConnection dbConnection, Dictionary<Type, string> createTablePropertiesConversion)
        {
            _internalRepository = new InternalRepository(typeof(T), dbConnection, createTablePropertiesConversion);
        }

        /// <summary>
        /// Creates a Repository of type T with a standard conversion table for the db types to c# types conversion
        /// </summary>
        /// <param name="dbConnection">Connection to database</param>
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

        /// <summary>
        /// Deletes entity
        /// </summary>
        /// <param name="value">Instance of entity type</param>
        public void Delete(T value)
        {
            _internalRepository.Delete(value);
        }

        /// <summary>
        /// Persists entity in database
        /// </summary>
        /// <param name="value">Instance of entity type</param>
        public void Save(T value)
        {
            _internalRepository.Save(value);
        }

        /// <summary>
        /// Gets instance by primary key from database
        /// </summary>
        /// <param name="primaryKey">Primary Key of entity type</param>
        /// <returns>Instance of entity type</returns>
        public T Get(P primaryKey)
        {
            return (T)_internalRepository.GetEntityByPrimaryKey(primaryKey);
        }

        /// <summary>
        /// Creates table in the database
        /// </summary>
        public void SetupTable()
        {
            _internalRepository.SetupTable();
        }

        /// <summary>
        /// Creates foreign keys in the database
        /// </summary>
        public void SetupForeignKeys()
        {
            _internalRepository.SetupForeignKeys();
        }

        /// <summary>
        /// Query for creating a custom query for a list of entity type
        /// </summary>
        /// <returns>A query action (filter) of entity type</returns>
        public QueryAction<T> CreateQuery()
        {
            return _internalRepository.CreateQuery<T>();
        }

        /// <summary>
        /// Executes a custom query for a list entity type
        /// </summary>
        /// <param name="queryGroup">Custom query of entity type</param>
        /// <returns></returns>
        public IEnumerable<T> Query(QueryGroup<T> queryGroup)
        {
            return _internalRepository.Query<T>(queryGroup);
        }
    }
}
