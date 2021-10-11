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
        private ModelEntity modelEntity;
        //private IDbConnection connection;
        public IDbConnection Connection { get; set; }

        public Repository(IDbConnection dbConnection, Dictionary<Type, string> createTablePropertiesConversion = null)
        {
            Connection = dbConnection;
            modelEntity = new ModelEntity(typeof(T));
            if(createTablePropertiesConversion != null)
            {
                _createTablePropertiesConversion = createTablePropertiesConversion;
            }
        }

        public Repository(Dictionary<Type, string> createTablePropertiesConversion = null)
        {
            modelEntity = new ModelEntity(typeof(T));
            if (createTablePropertiesConversion != null)
            {
                _createTablePropertiesConversion = createTablePropertiesConversion;
            }
        }

        public void Setup()
        {
            CreateTable();
        }

        private Dictionary<Type, string> _createTablePropertiesConversion = new Dictionary<Type, string>();
        public Dictionary<Type, string> CreateTablePropertiesConversion
        {
            get
            {
                return _createTablePropertiesConversion;
            }
        }

        private void CreateTable()
        {
            ModelEntity ent = modelEntity;
            // ModelEntity ent = new ModelEntity(value);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("CREATE TABLE IF NOT EXISTS " + ent.TableName);

            string create = "";

            for (int i = 0; i < ent.Fields.Length; i++)
            {
                if (i > 0) { create += ", "; }
                create += ent.Fields[i].ColumnName + " ";

                if (CreateTablePropertiesConversion.ContainsKey(ent.Fields[i]?.ColumnType ?? ent.Fields[i].Type))
                {
                    create += CreateTablePropertiesConversion[ent.Fields[i].ColumnType];
                }
                else
                {
                    create += ent.Fields[i].ColumnType.Name;
                }

                create += ent.Fields[i].IsNullable ? " NULL " : " NOT NULL ";
                create += ent.Fields[i].IsPrimaryKey ? "PRIMARY KEY" : "";
            }
            cmd.CommandText += (" (" + create + ") ");

            var result = cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public void Save(object value)
        {
            ModelEntity ent = new ModelEntity(value);

            IDbCommand cmd = Connection.CreateCommand();
            cmd.CommandText = ("INSERT INTO " + ent.TableName + " (");

            string update = "ON CONFLICT (" + ent.PrimaryKey.ColumnName + ") DO UPDATE SET ";
            string insert = "";

            IDataParameter p;
            bool first = true;
            for (int i = 0; i < ent.Fields.Length; i++)
            {
                if (i > 0) { cmd.CommandText += ", "; insert += ", "; }
                cmd.CommandText += ent.Fields[i].ColumnName;

                insert += (":v" + i.ToString());

                p = cmd.CreateParameter();
                p.ParameterName = (":v" + i.ToString());
                p.Value = ent.Fields[i].ToColumnType(ent.Fields[i].GetValue(value));
                cmd.Parameters.Add(p);

                if (!ent.Fields[i].IsPrimaryKey)
                {
                    if (first) { first = false; } else { update += ", "; }
                    update += (ent.Fields[i].ColumnName + " = " + (":w" + i.ToString()));

                    p = cmd.CreateParameter();
                    p.ParameterName = (":w" + i.ToString());
                    p.Value = ent.Fields[i].ToColumnType(ent.Fields[i].GetValue(value));
                    cmd.Parameters.Add(p);
                }
            }
            cmd.CommandText += (") VALUES (" + insert + ") " + update);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        public T Get(object pk)
        {
            return (T)CreateObject(typeof(T), pk);
        }

        /// <summary>Creates an object from a database reader.</summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="re">Reader.</param>
        /// <returns>Object.</returns>
        private object CreateObject(Type t, IDataReader re)
        {
            object rval = Activator.CreateInstance(t);
            ModelEntity modelEntity = new ModelEntity(t);
            foreach (ModelField i in modelEntity.Fields)
            {
                i.SetValue(rval, i.ToFieldType(re.GetValue(re.GetOrdinal(i.ColumnName))));
            }
            return rval;
        }


        /// <summary>Creates an instance by its primary keys.</summary>
        /// <param name="t">Type.</param>
        /// <param name="pk">Primary key.</param>
        /// <returns>Object.</returns>
        private object CreateObject(Type type, object primaryKey)
        {
            IDbCommand cmd = Connection.CreateCommand();
            ModelEntity modelEntity = new ModelEntity(type);
            cmd.CommandText = modelEntity.GetSQL() + " WHERE " + modelEntity.PrimaryKey.ColumnName + " = :pk";
            IDataParameter para = cmd.CreateParameter();
            para.ParameterName = (":pk");
            para.Value = primaryKey;
            cmd.Parameters.Add(para);

            object rval = null;
            IDataReader readerData = cmd.ExecuteReader();
            if (readerData.Read())
            {
                rval = CreateObject(type, readerData);
            }
            readerData.Close();
            cmd.Dispose();

            if (rval == null) { throw new Exception("No data."); }
            return rval;
        }
    }
}
