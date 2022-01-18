using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using WS2021.SWE3.OR_Mapper.Cache;
using WS2021.SWE3.OR_Mapper.CustomQuery;
using WS2021.SWE3.OR_Mapper.Exceptions;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper
{
    /// <summary>
    /// 
    /// </summary>
    class InternalRepository : IEntityInitializer
    {
        private Type _repositoryType;
        private EntityRegistry _entityRegistry = new EntityRegistry();
        private IDbCache _dbCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Type of entity which should be managed by the repository</param>
        /// <param name="dbConnection">Established Connection to a database</param>
        /// <param name="createTablePropertiesConversion">Conversion table for database types</param>
        /// <param name="dbCache">Cache used in repository</param>
        public InternalRepository(Type type, IDbConnection dbConnection, Dictionary<Type, string> createTablePropertiesConversion = null, IDbCache dbCache = null)
        {
            _repositoryType = type;
            Connection = dbConnection;
            modelEntity = _entityRegistry.GetModelEntity(_repositoryType);
            _dbCache = new DbCache(_entityRegistry);
            if (createTablePropertiesConversion != null)
            {
                _createTablePropertiesConversion = createTablePropertiesConversion;
            }
            if (dbCache != null)
            {
                _dbCache = dbCache;
            }
        }

        private ModelEntity modelEntity;
        public IDbConnection Connection { get; set; }

        /// <summary>
        /// Creation of Tables if they exists
        /// </summary>
        public void SetupTable()
        {
            CreateTable(modelEntity);
        }
        /// <summary>
        /// Creation of Foreign Keys if they exists
        /// </summary>
        public void SetupForeignKeys()
        {
            CreateForeignKeys(modelEntity);
        }

        private Dictionary<Type, string> _createTablePropertiesConversion = new Dictionary<Type, string>();
        public Dictionary<Type, string> CreateTablePropertiesConversion
        {
            get
            {
                return _createTablePropertiesConversion;
            }
        }

        /// <summary>
        /// Get DB Name of Column Type responding to the modelField
        /// </summary>
        /// <param name="modelField"></param>
        /// <returns></returns>
        private string GetColumnType(ModelField modelField)
        {
            if (CreateTablePropertiesConversion.ContainsKey(modelField?.ColumnType ?? modelField.Type))
            {
                return CreateTablePropertiesConversion[modelField.ColumnType];
            }
            else
            {
                return modelField.ColumnType.Name;
            }
        }

        /// <summary>
        /// Creates table in database accessed trough the connection.
        /// This method does not create any foreign keys.
        /// </summary>
        /// <param name="entity">Creates table for this entity</param>
        /// <exception cref="DbTypeConversionNotFound">If type of a filed of the entity cannot be found in the conversion table</exception>
        private void CreateTable(ModelEntity entity)
        {
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = ("CREATE TABLE IF NOT EXISTS " + entity.TableName);

            string create = "";

            for (int i = 0; i < entity.LocalFields.Length; i++)
            {
                if (entity.LocalFields[i].IsForeignKey && entity.LocalFields[i].IsEntity)
                {
                    ModelEntity modelEntityForeign = _entityRegistry.GetModelEntity(entity.LocalFields[i].Type);
                    CreateTable(modelEntityForeign);
                }
                if (i > 0) { create += ", "; }
                create += entity.LocalFields[i].ColumnName + " ";

                if (entity.LocalFields[i].IsForeignKey)
                {
                    ModelEntity modelEntityForeign = _entityRegistry.GetModelEntity(entity.LocalFields[i].Type);
                    if (!CreateTablePropertiesConversion.ContainsKey(modelEntityForeign.PrimaryKey.ColumnType))
                    {
                        throw new DbTypeConversionNotFound($"Error with converting db type: not found in conversion table - {modelEntityForeign.PrimaryKey.ColumnType}");
                    }
                    create += CreateTablePropertiesConversion[modelEntityForeign.PrimaryKey.ColumnType];
                }
                else
                {
                    create += GetColumnType(entity.LocalFields[i]);
                }

                create += entity.LocalFields[i].IsNullable ? " NULL " : " NOT NULL ";
                create += entity.LocalFields[i].IsPrimaryKey ? "PRIMARY KEY" : "";
            }
            command.CommandText += (" (" + create + ") ");

            var result = command.ExecuteNonQuery();
            command.Dispose();
            for (int i = 0; i < entity.ForeignFields.Length; i++)
            {
                ModelField modelFieldExternal = entity.ForeignFields[i];
                if (modelFieldExternal.IsManyToMany)
                {
                    IDbCommand commandManyToMany = Connection.CreateCommand();
                    commandManyToMany.CommandText += $"CREATE TABLE IF NOT EXISTS {modelFieldExternal.RemoteTableName}";
                    string cmdText = "";
                    cmdText += modelFieldExternal.RemoteTableColumnName + " " + GetColumnType(modelFieldExternal.Entity.PrimaryKey);
                    cmdText += ", ";
                    cmdText += modelFieldExternal.ColumnName + " " + GetColumnType(entity.PrimaryKey);
                    cmdText += ", ";
                    cmdText += "PRIMARY KEY ( " + modelFieldExternal.RemoteTableColumnName + ", " + modelFieldExternal.ColumnName + " )";
                    commandManyToMany.CommandText += (" (" + cmdText + ") ");
                    commandManyToMany.ExecuteNonQuery();
                    commandManyToMany.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates foreign keys in database accessed trough the connection.
        /// This method does not create any tables.
        /// </summary>
        /// <param name="entity">Creates foreign keys for this entity</param>
        private void CreateForeignKeys(ModelEntity entity)
        {
            for (int i = 0; i < entity.LocalFields.Length; i++)
            {
                if (entity.LocalFields[i].IsForeignKey && entity.LocalFields[i].IsEntity)
                {
                    ModelField modelFieldForeign = entity.LocalFields[i];
                    ModelEntity modelEntityForeign = _entityRegistry.GetModelEntity(modelFieldForeign.Type);

                    IDbCommand dropForeignKeys = Connection.CreateCommand();
                    dropForeignKeys.CommandText += $"ALTER TABLE {modelEntity.TableName} DROP CONSTRAINT IF EXISTS FK_{modelEntity.TableName}_{modelEntityForeign.TableName}";
                    dropForeignKeys.ExecuteNonQuery();
                    dropForeignKeys.Dispose();

                    IDbCommand createForeignKeys = Connection.CreateCommand();
                    createForeignKeys.CommandText += $"ALTER TABLE {modelEntity.TableName} ADD CONSTRAINT FK_{modelEntity.TableName}_{modelEntityForeign.TableName} FOREIGN KEY ({modelFieldForeign.ColumnName}) REFERENCES {modelEntityForeign.TableName}({modelEntityForeign.PrimaryKey.ColumnName})  ON UPDATE CASCADE ON DELETE CASCADE ";
                    createForeignKeys.ExecuteNonQuery();
                    createForeignKeys.Dispose();
                }
            }
            for (int i = 0; i < entity.ForeignFields.Length; i++)
            {
                ModelField modelFieldForeignMany = entity.ForeignFields[i];
                if (modelFieldForeignMany.IsManyToMany)
                {
                    IDbCommand dropForeignKeys = Connection.CreateCommand();
                    dropForeignKeys.CommandText += $"ALTER TABLE {modelFieldForeignMany.RemoteTableName} DROP CONSTRAINT IF EXISTS FK_{modelFieldForeignMany.RemoteTableName}_{modelEntity.TableName}";
                    dropForeignKeys.ExecuteNonQuery();
                    dropForeignKeys.Dispose();

                    IDbCommand createForeignKeys = Connection.CreateCommand();
                    createForeignKeys.CommandText += $"ALTER TABLE {modelFieldForeignMany.RemoteTableName} ADD CONSTRAINT FK_{modelFieldForeignMany.RemoteTableName}_{modelEntity.TableName} FOREIGN KEY ({modelFieldForeignMany.ColumnName}) REFERENCES {modelEntity.TableName}({entity.PrimaryKey.ColumnName}) ON UPDATE CASCADE ON DELETE CASCADE ";
                    createForeignKeys.ExecuteNonQuery();
                    createForeignKeys.Dispose();
                }
            }
        }

        public void Delete(object value)
        {
            ModelEntity entity = _entityRegistry.GetModelEntity(value);
            for (int i = 0; i < entity.ForeignFields.Length; i++)
            {
                if (entity.ForeignFields[i].IsManyToMany)
                {
                    DeleteManyToMany(entity.ForeignFields[i], entity.PrimaryKey.GetValue(value));
                }
            }
            DeleteRowByPrimaryKey(value, entity);
            _dbCache.RemoveValueWithDependencies(value);
        }


        private int DeleteManyToMany(ModelField modelField, object primaryKey)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("DELETE FROM " + modelField.RemoteTableName + " WHERE " + modelField.ColumnName + " = :pk");
            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ":pk";
            param.Value = primaryKey;
            cmd.Parameters.Add(param);
            int result = cmd.ExecuteNonQuery();
            cmd.Dispose();
            return result;
        }

        private void DeleteRowByPrimaryKey(object value, ModelEntity entity)
        {
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = ($"DELETE FROM {entity.TableName} WHERE {entity.PrimaryKey.ColumnName} = :pk");
            IDbDataParameter parameter = CreateParameterOfField(":pk", value, command, entity.PrimaryKey);
            command.Parameters.Add(parameter);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void Save(object value, ModelEntity parentEntity = null)
        {
            ModelEntity entity = _entityRegistry.GetModelEntity(value);
            SaveLocalFields(value, entity, parentEntity);
            for (int i = 0; i < entity.ForeignFields.Length; i++)
            {
                if(entity.ForeignFields[i].Type != parentEntity?.Type)
                {
                    SaveForeignFields(value, entity.ForeignFields[i], parentEntity);
                }
            }
            _dbCache.StoreValue(value);
        }

        private void SaveLocalFields(object value, ModelEntity entity, ModelEntity parentEntity = null)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("INSERT INTO " + entity.TableName + " (");
            string update = "ON CONFLICT (" + entity.PrimaryKey.ColumnName + ") DO UPDATE SET ";
            string insert = "";
            IDataParameter parameter;
            bool first = true;
            for (int i = 0; i < entity.LocalFields.Length; i++)
            {
                if (entity.LocalFields[i].IsForeignKey && entity.LocalFields[i].GetValue(value) != null && entity.LocalFields[i].Type != parentEntity?.Type)
                {
                    Save(entity.LocalFields[i].GetValue(value), entity);
                }
                if (i > 0) { cmd.CommandText += ", "; insert += ", "; }
                cmd.CommandText += entity.LocalFields[i].ColumnName;
                insert += ($":v{i}");
                parameter = CreateParameterOfField(($":v{i}"), value, cmd, entity.LocalFields[i]);
                cmd.Parameters.Add(parameter);
                if (!entity.LocalFields[i].IsPrimaryKey)
                {
                    if (first) { first = false; } else { update += ", "; }
                    update += (entity.LocalFields[i].ColumnName + " = " + ($":w{i}"));

                    parameter = CreateParameterOfField(($":w{i}"), value, cmd, entity.LocalFields[i]);
                    cmd.Parameters.Add(parameter);
                }
            }
            cmd.CommandText += ($") VALUES ({insert}) {update}");
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        //private void SaveLocalFields(object value, ModelEntity entity)
        //{
        //    IDbCommand cmd = Connection.CreateCommand();
        //    cmd.CommandText = ("INSERT INTO " + entity.TableName + " (");
        //    string update = "ON CONFLICT (" + entity.PrimaryKey.ColumnName + ") DO UPDATE SET ";
        //    string insert = "";
        //    IDataParameter parameter;
        //    bool first = true;
        //    for (int i = 0; i < entity.LocalFields.Length; i++)
        //    {
        //        if (entity.LocalFields[i].IsForeignKey && entity.LocalFields[i].GetValue(value) != null)
        //        {
        //            Save(entity.LocalFields[i].GetValue(value));
        //        }
        //        if (i > 0) { cmd.CommandText += ", "; insert += ", "; }
        //        cmd.CommandText += entity.LocalFields[i].ColumnName;
        //        insert += ($":v{i}");
        //        parameter = CreateParameterOfField(($":v{i}"), value, cmd, entity.LocalFields[i]);
        //        cmd.Parameters.Add(parameter);
        //        if (!entity.LocalFields[i].IsPrimaryKey)
        //        {
        //            if (first) { first = false; } else { update += ", "; }
        //            update += (entity.LocalFields[i].ColumnName + " = " + ($":w{i}"));

        //            parameter = CreateParameterOfField(($":w{i}"), value, cmd, entity.LocalFields[i]);
        //            cmd.Parameters.Add(parameter);
        //        }
        //    }
        //    cmd.CommandText += ($") VALUES ({insert}) {update}");
        //    cmd.ExecuteNonQuery();
        //    cmd.Dispose();
        //}

        void SaveForeignFields(object obj, ModelField modelField, ModelEntity parentEntity = null)
        {

            if(!modelField.IsForeignField) return;
            if(modelField.GetValue(obj) == null) return;
            
            Type innerType = modelField.Type.GetGenericArguments()[0];
            ModelEntity innerEntity = _entityRegistry.GetModelEntity(innerType);
            object primaryKey = modelField.Entity.PrimaryKey.ToColumnType(modelField.Entity.PrimaryKey.GetValue(obj));
            if (parentEntity != null && parentEntity.Type == innerEntity.Type) return;
            if (modelField.IsManyToMany)
            {
                SaveManyToManyRelation(obj, modelField, innerEntity, primaryKey);
            }
            else
            {
                SaveOneToManyRelation(obj, modelField, innerEntity, primaryKey);
            }
        }

        private void SaveOneToManyRelation(object obj, ModelField modelField, ModelEntity innerEntity, object primaryKey)
        {
            ModelField remoteField = innerEntity.GetFieldForColumn(modelField.ColumnName);
            if (remoteField.IsNullable)
            {
                try
                {
                    IDbCommand cmd = Connection.CreateCommand();
                    cmd.CommandText = ("UPDATE " + innerEntity.TableName + " SET " + modelField.ColumnName + " = NULL WHERE " + modelField.ColumnName + " = :fk");
                    IDataParameter param = cmd.CreateParameter();
                    param.ParameterName = ":fk";
                    param.Value = primaryKey;
                    cmd.Parameters.Add(param);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception) { }
            }

            if (modelField.GetValue(obj) != null)
            {
                IEnumerable fieldItemList = (IEnumerable) modelField.GetValue(obj);
                foreach (object listitem in fieldItemList)
                {
                    remoteField.SetValue(listitem, obj);
                    
                    IDbCommand cmd = Connection.CreateCommand();
                    cmd.CommandText = ("UPDATE " + innerEntity.TableName + " SET " + modelField.ColumnName + " = :fk WHERE " + innerEntity.PrimaryKey.ColumnName + " = :pk");
                    IDataParameter param = cmd.CreateParameter();
                    param.ParameterName = ":fk";
                    param.Value = primaryKey;
                    cmd.Parameters.Add(param);

                    param = cmd.CreateParameter();
                    param.ParameterName = ":pk";
                    param.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(listitem));
                    cmd.Parameters.Add(param);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
        }

        private void SaveManyToManyRelation(object obj, ModelField modelField, ModelEntity innerEntity, object primaryKey)
        {
            IDbCommand cmd;
            IDataParameter param;
            DeleteManyToMany(modelField, primaryKey);
            if (modelField.GetValue(obj) != null)
            {
                IEnumerable list = (IEnumerable)modelField.GetValue(obj);
                foreach (object listObj in list)
                {
                    cmd = Connection.CreateCommand();
                    cmd.CommandText = ("INSERT INTO " + modelField.RemoteTableName + " (" + modelField.ColumnName + ", " + modelField.RemoteTableColumnName + ") VALUES (:pk, :fk)");
                    param = cmd.CreateParameter();
                    param.ParameterName = ":pk";
                    param.Value = primaryKey;
                    cmd.Parameters.Add(param);

                    param = cmd.CreateParameter();
                    param.ParameterName = ":fk";
                    param.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(listObj));
                    cmd.Parameters.Add(param);

                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
        }

        private IDbDataParameter CreateParameterOfField(string paramName, object value, IDbCommand dbCommand)
        {
            var parameter = dbCommand.CreateParameter();
            parameter.ParameterName = paramName;
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value;
            }
            return parameter;
        }

        private IDbDataParameter CreateParameterOfField(string paramName, object value, IDbCommand dbCommand, ModelField currentField)
        {
            object realValue = currentField.ToColumnType(currentField.GetValue(value));
            return CreateParameterOfField(paramName, realValue, dbCommand);
        }

        public object GetEntityByPrimaryKey(object primaryKey)
        {
            return InitEntityFromDb(_repositoryType, primaryKey);
        }

        public object InitEntityFromDb(Type type, object primaryKey)
        {
            object resultValue = _dbCache.GetValue(type, primaryKey);
            if (resultValue != null) { return resultValue; }
            IDbCommand command = Connection.CreateCommand();
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(type);
            command.CommandText = modelEntity.GetSQLLocalFields() + " WHERE " + modelEntity.PrimaryKey.ColumnName + " = :pk";
            IDataParameter para = command.CreateParameter();
            para.ParameterName = (":pk");
            para.Value = primaryKey;
            command.Parameters.Add(para);
            IDataReader readerData = command.ExecuteReader();
            Dictionary<string, object> columnValuePairs = DataReaderToDictionary(readerData, modelEntity);
            readerData.Close();
            if (columnValuePairs != null && columnValuePairs.Count > 0)
            {
                resultValue = InitEntityFromDictionary(type, columnValuePairs);
            }
            command.Dispose();
            
            return resultValue;
        }

        private Dictionary<string, object> DataReaderToDictionary(IDataReader dataReader, ModelEntity entity)
        {
            Dictionary<string, object> columnValuePairs = new();
            if (dataReader.Read())
            {
                foreach (ModelField modelField in entity.LocalFields)
                {
                    columnValuePairs.Add(modelField.ColumnName, dataReader.GetValue(dataReader.GetOrdinal(modelField.ColumnName)));
                }
            }
            return columnValuePairs;
        }

        public object InitEntityFromDictionary(Type type, Dictionary<string, object> columnValuePairs)
        {
            if(columnValuePairs == null || columnValuePairs.Count == 0) { return null; } 
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(type);
            object primaryKey = modelEntity.PrimaryKey.ToFieldType(columnValuePairs[modelEntity.PrimaryKey.ColumnName], this);
            object resultValue = _dbCache.GetValue(type, primaryKey);
            if (resultValue == null)
            {
                resultValue = Activator.CreateInstance(type);
                modelEntity.PrimaryKey.SetValue(resultValue, primaryKey);
                _dbCache.StoreValue(resultValue);
            }
            foreach (ModelField inField in modelEntity.LocalFields)
            {
                inField.SetValue(resultValue, inField.ToFieldType(columnValuePairs[inField.ColumnName], this));
            }
            foreach (ModelField modelField in modelEntity.ForeignFields)
            {
                modelField.SetValue(resultValue, InitForeignField(modelField, Activator.CreateInstance(modelField.Type), resultValue));
            }
            _dbCache.StoreValue(resultValue);
            return resultValue;
        }

        private object InitForeignField(ModelField field, object list, object value)
        {
            Type listType = field.Type.GenericTypeArguments[0];
            List<Dictionary<string, object>> objectsList = GetListOfField(field, value, listType);
            foreach (Dictionary<string, object> columnValuePair in objectsList)
            {
                var obj = InitEntityFromDictionary(listType, columnValuePair);
                list.GetType().GetMethod("Add").Invoke(list, new object[] { obj });
            }
            
            return list;
        }

        private List<Dictionary<string, object>> GetListOfField(ModelField field, object value, Type listType)
        {
            List<Dictionary<string, object>> objectsList = new List<Dictionary<string, object>>();
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(listType);
            IDbCommand command = Connection.CreateCommand();
            if (field.IsManyToMany)
            {
                command.CommandText = modelEntity.GetSQLLocalFields() +
                                  " WHERE ID IN (SELECT " + field.RemoteTableColumnName + " FROM " + field.RemoteTableName + " WHERE " + field.ColumnName + " = :fk)";
            }
            else
            {
                command.CommandText = modelEntity.GetSQLLocalFields() + " WHERE " + field.ColumnName + " = :fk";
            }
            IDataParameter param = command.CreateParameter();
            param.ParameterName = ":fk";
            param.Value = field.Entity.PrimaryKey.GetValue(value);
            command.Parameters.Add(param);
            IDataReader reader = command.ExecuteReader();
            Dictionary<string, object> columnValuePairs;
            do
            {
                columnValuePairs = DataReaderToDictionary(reader, modelEntity);
                if (columnValuePairs.Count > 0)
                {
                    objectsList.Add(columnValuePairs);
                }
            } while (columnValuePairs != null && columnValuePairs.Count > 0);
            reader.Close();
            reader.Dispose();
            command.Dispose();
            return objectsList;
        }

        /// <summary>
        /// Creates a custom query for a list of entity type
        /// </summary>
        /// <typeparam name="T">Type of entity to filter</typeparam>
        /// <returns>A query action to add a filter</returns>
        public QueryAction<T> CreateQuery<T>()
        {
            return new QueryAction<T>(_entityRegistry);
        }

        /// <summary>
        /// Executes a custom query for a list entity type
        /// </summary>
        /// <typeparam name="T">Type of entity to filter</typeparam>
        /// <param name="queryGroup">Customer query to execute</param>
        /// <returns>A list of entities filtered by the query</returns>
        public IEnumerable<T> Query<T>(QueryGroup<T> queryGroup)
        {
            List<T> list = new List<T>();
            Type listType = typeof(T);
            string whereClause = queryGroup.GetWhereClause();
            var paramsWhere = queryGroup.GetWhereClauseParams();
            IDbCommand command = Connection.CreateCommand();
            ModelEntity entity = _entityRegistry.GetModelEntity(listType);
            command.CommandText = entity.GetSQLLocalFields() + " WHERE " + whereClause;
            foreach(var para in paramsWhere)
            {
                command.Parameters.Add(CreateParameterOfField(para.Item1, para.Item2, command));
            }
            IDataReader reader = command.ExecuteReader();
            List<Dictionary<string, object>> objectsList = new();
            Dictionary<string, object> columnValuePairs;
            do
            {
                columnValuePairs = DataReaderToDictionary(reader, modelEntity);
                if (columnValuePairs.Count > 0)
                {
                    objectsList.Add(columnValuePairs);
                }
            } while (columnValuePairs != null && columnValuePairs.Count > 0);
            reader.Close();
            foreach (Dictionary<string, object> columnValuePair in objectsList)
            {
                var obj = InitEntityFromDictionary(listType, columnValuePair);
                list.GetType().GetMethod("Add").Invoke(list, new object[] { obj });
            }
            reader.Close();
            reader.Dispose();
            command.Dispose();
            return list;
        }

        public object InitEntity(Type type, object primaryKey)
        {
            return InitEntityFromDb(type, primaryKey);
        }
    }
}
