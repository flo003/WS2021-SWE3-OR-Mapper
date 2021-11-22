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
    class InternalRepository
    {
        private Type repositoryType;
        private EntityRegistry entityRegistry = new EntityRegistry();
        private DbChache _dbChache;

        public InternalRepository(Type type, IDbConnection dbConnection, Dictionary<Type, string> createTablePropertiesConversion = null)
        {
            repositoryType = type;
            Connection = dbConnection;
            modelEntity = entityRegistry.GetModelEntity(repositoryType);
            if (createTablePropertiesConversion != null)
            {
                _createTablePropertiesConversion = createTablePropertiesConversion;
            }
            _dbChache = new DbChache(entityRegistry);
        }

        public InternalRepository(Type type, Dictionary<Type, string> createTablePropertiesConversion = null)
        {
            repositoryType = type;
            modelEntity = entityRegistry.GetModelEntity(repositoryType);
            if (createTablePropertiesConversion != null)
            {
                _createTablePropertiesConversion = createTablePropertiesConversion;
            }
        }

        private ModelEntity modelEntity;
        //private IDbConnection connection;
        public IDbConnection Connection { get; set; }

        public void SetupTable()
        {
            CreateTable(modelEntity);
        }
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

        private void CreateTable(ModelEntity entity)
        {
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = ("CREATE TABLE IF NOT EXISTS " + entity.TableName);

            string create = "";

            for (int i = 0; i < entity.LocalFields.Length; i++)
            {
                if (entity.LocalFields[i].IsForeignKey && entity.LocalFields[i].IsEntity)
                {
                    ModelEntity modelEntityForeign = entityRegistry.GetModelEntity(entity.LocalFields[i].Type);
                    CreateTable(modelEntityForeign);
                }
                if (i > 0) { create += ", "; }
                create += entity.LocalFields[i].ColumnName + " ";

                if (entity.LocalFields[i].IsForeignKey)
                {
                    ModelEntity modelEntityForeign = entityRegistry.GetModelEntity(entity.LocalFields[i].Type);
                    if (!CreateTablePropertiesConversion.ContainsKey(modelEntityForeign.PrimaryKey.ColumnType))
                    {
                        throw new NotImplementedException(); // throw better named exceptions
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

        private void CreateForeignKeys(ModelEntity entity)
        {
            for (int i = 0; i < entity.LocalFields.Length; i++)
            {
                if (entity.LocalFields[i].IsForeignKey && entity.LocalFields[i].IsEntity)
                {
                    ModelField modelFieldForeign = entity.LocalFields[i];
                    ModelEntity modelEntityForeign = entityRegistry.GetModelEntity(modelFieldForeign.Type);
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
                    IDbCommand createForeignKeys = Connection.CreateCommand();
                    createForeignKeys.CommandText += $"ALTER TABLE {modelFieldForeignMany.RemoteTableName} ADD CONSTRAINT FK_{modelFieldForeignMany.RemoteTableName}_{modelEntity.TableName} FOREIGN KEY ({modelFieldForeignMany.ColumnName}) REFERENCES {modelEntity.TableName}({entity.PrimaryKey.ColumnName}) ON UPDATE CASCADE ON DELETE CASCADE ";
                    createForeignKeys.ExecuteNonQuery();
                    createForeignKeys.Dispose();
                }
            }
        }

        public void Delete(object value)
        {
            ModelEntity entity = entityRegistry.GetModelEntity(value);
            IDbCommand command = Connection.CreateCommand();
            command.CommandText = ($"DELETE FROM {entity.TableName} WHERE {entity.PrimaryKey.ColumnName} = :pk");
            IDbDataParameter parameter = CreateParameterOfField(":pk", value, command, entity.PrimaryKey);
            command.Parameters.Add(parameter);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public void Save(object value)
        {
            ModelEntity entity = entityRegistry.GetModelEntity(value);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("INSERT INTO " + entity.TableName + " (");

            string update = "ON CONFLICT (" + entity.PrimaryKey.ColumnName + ") DO UPDATE SET ";
            string insert = "";

            IDataParameter parameter;
            bool first = true;
            for (int i = 0; i < entity.LocalFields.Length; i++)
            {
                if (entity.LocalFields[i].IsForeignKey && entity.LocalFields[i].GetValue(value) != null)
                {
                    Save(entity.LocalFields[i].GetValue(value));
                }
                if (i > 0) { cmd.CommandText += ", "; insert += ", "; }
                cmd.CommandText += entity.LocalFields[i].ColumnName;

                insert += (":v" + i.ToString());

                parameter = CreateParameterOfField(($":v{i}"), value, cmd, entity.LocalFields[i]);
                cmd.Parameters.Add(parameter);

                if (!entity.LocalFields[i].IsPrimaryKey)
                {
                    if (first) { first = false; } else { update += ", "; }
                    update += (entity.LocalFields[i].ColumnName + " = " + (":w" + i.ToString()));

                    parameter = CreateParameterOfField(($":w{i}"), value, cmd, entity.LocalFields[i]);
                    cmd.Parameters.Add(parameter);
                }
            }
            cmd.CommandText += (") VALUES (" + insert + ") " + update);
            cmd.ExecuteNonQuery();
            cmd.Dispose();

            for (int i = 0; i < entity.ForeignFields.Length; i++)
            {
                UpdateReferences(value, entity.ForeignFields[i]);
            }

            _dbChache.StoreValue(value);
        }

        void UpdateReferences(object obj, ModelField modelField)
        {
            ModelEntity entity = entityRegistry.GetModelEntity(obj);
            if (!modelField.IsForeignField) return;

            Type innerType = modelField.Type.GetGenericArguments()[0];
            ModelEntity innerEntity = entityRegistry.GetModelEntity(innerType);
            object primaryKey = modelField.Entity.PrimaryKey.ToColumnType(modelField.Entity.PrimaryKey.GetValue(obj));
            if (modelField.IsManyToMany)
            {
                IDbCommand cmd = Connection.CreateCommand();
                cmd.CommandText = ("DELETE FROM " + modelField.RemoteTableName + " WHERE " + modelField.ColumnName + " = :pk");
                IDataParameter param = cmd.CreateParameter();
                param.ParameterName = ":pk";
                param.Value = primaryKey;
                cmd.Parameters.Add(param);

                cmd.ExecuteNonQuery();
                cmd.Dispose();

                if (modelField.GetValue(obj) != null)
                {
                    foreach (object i in (IEnumerable)modelField.GetValue(obj))
                    {
                        cmd = Connection.CreateCommand();
                        cmd.CommandText = ("INSERT INTO " + modelField.RemoteTableName + "(" + modelField.ColumnName + ", " + modelField.RemoteTableColumnName + ") VALUES (:pk, :fk)");
                        param = cmd.CreateParameter();
                        param.ParameterName = ":pk";
                        param.Value = primaryKey;
                        cmd.Parameters.Add(param);

                        param = cmd.CreateParameter();
                        param.ParameterName = ":fk";
                        param.Value = innerEntity.PrimaryKey.ToColumnType(innerEntity.PrimaryKey.GetValue(i));
                        cmd.Parameters.Add(param);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
            }
            else
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
                    foreach (object listitem in (IEnumerable)modelField.GetValue(obj))
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

        public object Get(object primaryKey)
        {
            return InitObject(repositoryType, primaryKey);
        }

        internal object SearchCache(Type type, object primaryKey)
        {
            object value;
            if ((value = _dbChache.GetValue(type, primaryKey)) != null)
            {
                return value;
            }
            return null;
        }

        public object InitObject(Type type, Dictionary<string, object> columnValuePairs)
        {
            ModelEntity modelEntity = entityRegistry.GetModelEntity(type);
            object resultValue = SearchCache(type, modelEntity.PrimaryKey.ToFieldType(columnValuePairs[modelEntity.PrimaryKey.ColumnName], this));
            bool foundInChache = true;
            if (resultValue == null)
            {
                foundInChache = false;
                _dbChache.StoreValue((resultValue = Activator.CreateInstance(type)));
            }
            foreach (ModelField inField in modelEntity.LocalFields)
            {
                inField.SetValue(resultValue, inField.ToFieldType(columnValuePairs[inField.ColumnName], this));
            }
            if (!foundInChache)
            {
                foreach (ModelField modelField in modelEntity.ForeignFields)
                {
                    modelField.SetValue(resultValue, FillList(modelField, Activator.CreateInstance(modelField.Type), resultValue));
                }
            }
            return resultValue;
        }

        public object InitObject(Type type, object primaryKey)
        {
            object resultValue = SearchCache(type, primaryKey);

            if (resultValue == null)
            {
                IDbCommand command = Connection.CreateCommand();
                ModelEntity modelEntity = entityRegistry.GetModelEntity(type);
                command.CommandText = modelEntity.GetSQLLocalFields() + " WHERE " + modelEntity.PrimaryKey.ColumnName + " = :pk";

                IDataParameter para = command.CreateParameter();
                para.ParameterName = (":pk");
                para.Value = primaryKey;
                command.Parameters.Add(para);

                IDataReader readerData = command.ExecuteReader();
                Dictionary<string, object> columnValuePairs = DataReaderToDictionary(readerData, modelEntity);
                readerData.Close();
                resultValue = InitObject(type, columnValuePairs);
                command.Dispose();
            }
            if (resultValue == null) { throw new Exception("No data."); }
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

        private object FillList(ModelField field, object list, object value)
        {
            IDbCommand command = Connection.CreateCommand();
            Type listType = field.Type.GenericTypeArguments[0];
            ModelEntity modelEntity = entityRegistry.GetModelEntity(listType);
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
            List<Dictionary<string, object>> objectsList = new();
            Dictionary<string, object> columnValuePairs = null;
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
                var obj = InitObject(listType, columnValuePair);
                list.GetType().GetMethod("Add").Invoke(list, new object[] { obj });
            }
            reader.Close();
            reader.Dispose();
            command.Dispose();
            return list;
        }

        public QueryAction CreateQuery()
        {
            return new QueryAction(entityRegistry);
        }

        public List<T> Query<T>(QueryGroup queryGroup)
        {
            List<T> list = new List<T>();
            Type listType = typeof(T);
            string whereClause = queryGroup.GetWhereClause();
            var paramsWhere = queryGroup.GetWhereClauseParams();
            IDbCommand command = Connection.CreateCommand();
            ModelEntity entity = entityRegistry.GetModelEntity(listType);
            command.CommandText = entity.GetSQLLocalFields() + " WHERE " + whereClause;
            foreach(var para in paramsWhere)
            {
                command.Parameters.Add(CreateParameterOfField(para.Item1, para.Item2, command));
            }
            IDataReader reader = command.ExecuteReader();
            List<Dictionary<string, object>> objectsList = new();
            Dictionary<string, object> columnValuePairs = null;
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
                var obj = InitObject(listType, columnValuePair);
                list.GetType().GetMethod("Add").Invoke(list, new object[] { obj });
            }
            reader.Close();
            reader.Dispose();
            command.Dispose();
            return list;
        }
    }
}
