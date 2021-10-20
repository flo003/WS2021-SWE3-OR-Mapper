using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper
{
    public class Repository<T>
    {
        private static Dictionary<Type, ModelEntity> entities = new Dictionary<Type, ModelEntity>();
        internal ModelEntity GetModelEntity(object obj)
        {
            return GetModelEntity(obj.GetType());
        }
        internal ModelEntity GetModelEntity(Type type)
        {
            if (entities.ContainsKey(type))
            {
                return entities[type];
            }
            ModelEntity modelEntity = new ModelEntity(type);
            entities.Add(type, modelEntity);
            return modelEntity;
        }


        private ModelEntity modelEntity;
        //private IDbConnection connection;
        public IDbConnection Connection { get; set; }

        public Repository(IDbConnection dbConnection, Dictionary<Type, string> createTablePropertiesConversion = null)
        {
            Connection = dbConnection;
            modelEntity = GetModelEntity(typeof(T));
            if (createTablePropertiesConversion != null)
            {
                _createTablePropertiesConversion = createTablePropertiesConversion;
            }
        }

        public Repository(Dictionary<Type, string> createTablePropertiesConversion = null)
        {
            modelEntity = GetModelEntity(typeof(T));
            if (createTablePropertiesConversion != null)
            {
                _createTablePropertiesConversion = createTablePropertiesConversion;
            }
        }

        public void Setup()
        {
            CreateTable(modelEntity);
        }

        private Dictionary<Type, string> _createTablePropertiesConversion = new Dictionary<Type, string>();
        public Dictionary<Type, string> CreateTablePropertiesConversion
        {
            get
            {
                return _createTablePropertiesConversion;
            }
        }


        private void CreateTable(ModelEntity entity)
        {
            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("CREATE TABLE IF NOT EXISTS " + entity.TableName);

            string create = "";

            for (int i = 0; i < entity.InternalFields.Length; i++)
            {
                if (entity.InternalFields[i].IsForeignKey && entity.InternalFields[i].IsEntity)
                {
                    ModelEntity modelEntityForeign = GetModelEntity(entity.InternalFields[i].Type);
                    CreateTable(modelEntityForeign);
                }
                if (i > 0) { create += ", "; }
                create += entity.InternalFields[i].ColumnName + " ";

                if (entity.InternalFields[i].IsForeignKey)
                {
                    ModelEntity modelEntityForeign = GetModelEntity(entity.InternalFields[i].Type);
                    if (!CreateTablePropertiesConversion.ContainsKey(modelEntityForeign.PrimaryKey.ColumnType))
                    {
                        throw new NotImplementedException();// throw better named exceptions
                    }
                    create += CreateTablePropertiesConversion[modelEntityForeign.PrimaryKey.ColumnType];
                }
                else if (CreateTablePropertiesConversion.ContainsKey(entity.InternalFields[i]?.ColumnType ?? entity.InternalFields[i].Type))
                {
                    create += CreateTablePropertiesConversion[entity.InternalFields[i].ColumnType];
                }
                else
                {
                    create += entity.InternalFields[i].ColumnType.Name;
                }

                create += entity.InternalFields[i].IsNullable ? " NULL " : " NOT NULL ";
                create += entity.InternalFields[i].IsPrimaryKey ? "PRIMARY KEY" : "";
            }
            cmd.CommandText += (" (" + create + ") ");

            var result = cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        public void Delete(object value)
        {
            ModelEntity entity = GetModelEntity(value);
        }

        public void Save(object value)
        {
            ModelEntity entity = GetModelEntity(value);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("INSERT INTO " + entity.TableName + " (");

            string update = "ON CONFLICT (" + entity.PrimaryKey.ColumnName + ") DO UPDATE SET ";
            string insert = "";

            IDataParameter para;
            bool first = true;
            for (int i = 0; i < entity.InternalFields.Length; i++)
            {
                if (entity.InternalFields[i].IsForeignKey && entity.InternalFields[i].GetValue(value) != null)
                {
                    Save(entity.InternalFields[i].GetValue(value));
                }
                if (i > 0) { cmd.CommandText += ", "; insert += ", "; }
                cmd.CommandText += entity.InternalFields[i].ColumnName;

                insert += (":v" + i.ToString());

                para = CreateParameterOfField((":v" + i.ToString()), value, entity, cmd, i);
                cmd.Parameters.Add(para);

                if (!entity.InternalFields[i].IsPrimaryKey)
                {
                    if (first) { first = false; } else { update += ", "; }
                    update += (entity.InternalFields[i].ColumnName + " = " + (":w" + i.ToString()));

                    para = CreateParameterOfField((":w" + i.ToString()), value, entity, cmd, i);
                    cmd.Parameters.Add(para);
                }
            }
            cmd.CommandText += (") VALUES (" + insert + ") " + update);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            for (int i = 0; i < entity.ExternalFields.Length; i++)
            {

            }
        }

        private IDbDataParameter CreateParameterOfField(string paramName, object value, ModelEntity entity, IDbCommand dbCommand, int currentFieldIndex)
        {
            var parameter = dbCommand.CreateParameter();
            parameter.ParameterName = paramName;
            parameter.Value = entity.InternalFields[currentFieldIndex].ToColumnType(entity.InternalFields[currentFieldIndex].GetValue(value));
            return parameter;
        }

        public T Get(object pk)
        {
            return (T)InitObject(typeof(T), pk, null);
        }

        internal object SearchCache(Type type, object primaryKey, ICollection<object> localCache)
        {
            if (localCache != null)
            {
                foreach (object obj in localCache)
                {
                    if (obj.GetType() != type) continue;

                    if (GetModelEntity(type).PrimaryKey.GetValue(obj).Equals(primaryKey)) { return obj; }
                }
            }
            return null;
        }

        public object InitObject(Type type, IDataReader reader, ICollection<object> localCache)
        {
            object rval = Activator.CreateInstance(type);
            ModelEntity modelEntity = GetModelEntity(type);
            foreach (ModelField i in modelEntity.InternalFields)
            {
                i.SetValue(rval, i.ToFieldType<T>(reader.GetValue(reader.GetOrdinal(i.ColumnName)), localCache ,this));
            }
            return rval;
        }

        public object InitObject(Type type, Dictionary<string, object> columnValuePairs, ICollection<object> localCache)
        {
            ModelEntity modelEntity = GetModelEntity(type);
            object rval = SearchCache(type, modelEntity.PrimaryKey.ToFieldType(columnValuePairs[modelEntity.PrimaryKey.ColumnName]), localCache);

            if (rval == null)
            {
                if (localCache == null) { localCache = new List<object>(); }
                localCache.Add(rval = Activator.CreateInstance(type));
            }

            foreach (ModelField i in modelEntity.InternalFields)
            {
                i.SetValue(rval, i.ToFieldType<T>(columnValuePairs[i.ColumnName], localCache, this));
            }

            foreach (ModelField modelField in modelEntity.ExternalFields)
            {
                modelField.SetValue(rval, FillList(modelField, Activator.CreateInstance(modelField.Type), rval, localCache));
            }
            return rval;
        }

        public object InitObject(Type type, object primaryKey, ICollection<object> localCache)
        {
            object resultValue = SearchCache(type, primaryKey, localCache);

            if (resultValue == null)
            {
                IDbCommand command = Connection.CreateCommand();
                ModelEntity modelEntity = GetModelEntity(type);
                command.CommandText = modelEntity.GetSQLInternalFields() + " WHERE " + modelEntity.PrimaryKey.ColumnName + " = :pk";

                IDataParameter para = command.CreateParameter();
                para.ParameterName = (":pk");
                para.Value = primaryKey;
                command.Parameters.Add(para);

                IDataReader readerData = command.ExecuteReader();
                Dictionary<string, object> columnValuePairs = DataReaderToDictionary(readerData, modelEntity);
                readerData.Close();
                resultValue = InitObject(type, columnValuePairs, localCache);
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
                foreach (ModelField modelField in entity.InternalFields)
                {
                    columnValuePairs.Add(modelField.ColumnName, dataReader.GetValue(dataReader.GetOrdinal(modelField.ColumnName)));
                }
            }
            return columnValuePairs;
        }

        private object FillList(ModelField field, object list, object value, ICollection<object> localCache)
        {
            IDbCommand cmd = Connection.CreateCommand();
            Type listType = field.Type.GenericTypeArguments[0];
            ModelEntity modelEntity = GetModelEntity(listType);
            if (field.IsManyToMany)
            {
                cmd.CommandText = modelEntity.GetSQLInternalFields() +
                                  " WHERE ID IN (SELECT " + field.RemoteTableColumnName + " FROM " + field.RemoteTableName + " WHERE " + field.ColumnName + " = :fk)";
            }
            else
            {   
                cmd.CommandText = modelEntity.GetSQLInternalFields() + " WHERE " + field.ColumnName + " = :fk";
            }

            IDataParameter param = cmd.CreateParameter();
            param.ParameterName = ":fk";
            param.Value = field.Entity.PrimaryKey.GetValue(value);
            cmd.Parameters.Add(param);

            IDataReader reader = cmd.ExecuteReader();
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
            foreach(Dictionary<string, object> columnValuePair in objectsList) {
                var obj = InitObject(listType, columnValuePair, localCache);
                list.GetType().GetMethod("Add").Invoke(list, new object[] { obj });
            }
            reader.Close();
            reader.Dispose();
            cmd.Dispose();

            return list;
        }
    }
}
