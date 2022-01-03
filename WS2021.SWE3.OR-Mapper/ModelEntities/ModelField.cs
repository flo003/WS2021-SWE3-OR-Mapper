using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.ModelEntities
{
    internal class ModelField
    {
        public ModelField(ModelEntity modelEntity)
        {
                Entity = modelEntity;
        }

        public MemberInfo Member { get; set; }
        public Type Type { 
            get
            {
                if(Member is PropertyInfo)
                {
                    return (Member as PropertyInfo).PropertyType;
                }
                throw new NotSupportedException();
            }
        }
        public ModelEntity Entity { get; set; }
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public DbType ColumnDbType { get; set; }
        public bool IsNullable { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsEntity { get; set; } = false;
        public bool IsForeignField { get; set; } = false;
        public string RemoteTableName { get; internal set; }
        public string RemoteTableColumnName { get; internal set; }
        public bool IsManyToMany { get; set; } = false;
        
        public object ToColumnType(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }
            if (IsForeignKey)
            {
                ModelEntity modelEntityForeign = new ModelEntity(Type);
                return modelEntityForeign.PrimaryKey.ToColumnType(modelEntityForeign.PrimaryKey.GetValue(value));
            }
            if (Type == ColumnType) { return value; }
            if (value is bool)
            {
                if (ColumnType == typeof(int)) { return (((bool)value) ? 1 : 0); }
                if (ColumnType == typeof(short)) { return (short)(((bool)value) ? 1 : 0); }
                if (ColumnType == typeof(long)) { return (long)(((bool)value) ? 1 : 0); }
            }
            if (Type.IsEnum) { return (Int32) value; }
            return value;
        }

        public object ToFieldType(object value, InternalRepository repository)
        {
            if (value == DBNull.Value)
            {
                return null;
            }
            if (IsForeignKey)
            {
                return repository.InitEntity(Type, value);
            }
            if (Type == typeof(bool))
            {
                if (value is int int1) { return (int1 != 0); }
                if (value is short short1) { return (short1 != 0); }
                if (value is long long1) { return (long1 != 0); }
            }
            if (Type == typeof(short)) { return Convert.ToInt16(value); }
            if (Type == typeof(int)) { return Convert.ToInt32(value); }
            if (Type == typeof(long)) { return Convert.ToInt64(value); }

            if (Type.IsEnum) { return Enum.ToObject(Type, Convert.ToInt32(value)); }

            return value;
        }

        public object GetValue(object obj)
        {
            if (Member is PropertyInfo info) { return info.GetValue(obj); }

            throw new NotSupportedException("Member type not supported.");
        }

        public void SetValue(object obj, object value)
        {
            if (Member is PropertyInfo)
            {
                ((PropertyInfo)Member).SetValue(obj, value);
                return;
            }

            throw new NotSupportedException("Member type not supported.");
        }
    }
}
