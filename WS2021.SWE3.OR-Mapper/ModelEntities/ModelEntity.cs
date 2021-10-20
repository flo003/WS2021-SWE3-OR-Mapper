using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelAttributes;

namespace WS2021.SWE3.OR_Mapper.ModelEntities
{
    internal class ModelEntity
    {
        public ModelEntity(object entity)
        {
            Type entityType = null;
            if (entity is Type)
            {
                entityType = entity as Type;
            }
            else
            {
                entityType = entity.GetType();
            }
            Type = entityType;
            var entityAttribute = entityType.GetCustomAttribute(typeof(EntityAttribute)) as EntityAttribute;
            if (entityAttribute != null && entityAttribute is EntityAttribute){
               TableName = entityAttribute?.TableName ?? entityType.Name;
            }
            else
            {
                TableName = entityType.Name;
            }
            
            var propertyInfos = entityType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var fields = new List<ModelField>();
            foreach(var propertyInfo in propertyInfos)
            {
                if(propertyInfo.CanWrite && propertyInfo.CanRead)
                {
                    ModelField newModelField = CreateModelField(propertyInfo);
                    if (newModelField != null)
                    {
                        fields.Add(newModelField);
                        if (newModelField.IsPrimaryKey)
                        {
                            PrimaryKey = newModelField;
                        }
                    }
                }
            }
            Fields = fields.ToArray();
            InternalFields = Fields.Where((field) => field.IsExternal == false).ToArray();
            ExternalFields = Fields.Where((field) => field.IsExternal == true).ToArray();
        }

        private ModelField CreateModelField(PropertyInfo propertyInfo)
        {
            ModelField modelField = new ModelField(this);
            modelField.Member = propertyInfo;
            IgnoreAttribute ignoreAttribute = modelField.Member.GetCustomAttribute(typeof(IgnoreAttribute)) as IgnoreAttribute;
            if (ignoreAttribute != null && ignoreAttribute is IgnoreAttribute)
            {
                return null;
            }
            FieldAttribute fieldAttribute = modelField.Member.GetCustomAttribute(typeof(FieldAttribute)) as FieldAttribute;
            if (fieldAttribute != null && fieldAttribute is FieldAttribute)
            {
                modelField.ColumnName = fieldAttribute?.ColumnName ?? propertyInfo.Name;
                modelField.ColumnType = fieldAttribute?.ColumnType ?? propertyInfo.PropertyType;
            }
            else
            {
                modelField.ColumnName = propertyInfo.Name;
                modelField.ColumnType = propertyInfo.PropertyType;
            }

            PrimaryKeyAttribute primaryAttribute = modelField.Member.GetCustomAttribute(typeof(PrimaryKeyAttribute)) as PrimaryKeyAttribute;
            if (primaryAttribute != null && primaryAttribute is PrimaryKeyAttribute)
            {
                modelField.IsPrimaryKey = true;
            }

            ForeignKeyAttribute foreignKeyAttribute = modelField.Member.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;
            if (foreignKeyAttribute != null && foreignKeyAttribute is ForeignKeyAttribute)
            {
                modelField.IsForeignKey = true;
                modelField.IsExternal = typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType);
                if (modelField.IsExternal)
                {
                    modelField.RemoteTableName = foreignKeyAttribute.RemoteTableName;
                    modelField.RemoteTableColumnName = foreignKeyAttribute.RemoteTableColumnName;
                }
            }

            EntityAttribute entityAttribute = modelField.Type.GetCustomAttribute(typeof(EntityAttribute)) as EntityAttribute;
            if (entityAttribute != null && entityAttribute is EntityAttribute)
            {
                modelField.IsEntity = true;
            }

            return modelField;
        }

        public Type Type { get; private set; }
        public string TableName { get; private set; }
        public ModelField[] Fields { get; private set; }
        public ModelField[] ExternalFields { get; private set; }
        public ModelField[] InternalFields { get; private set; }
        public ModelField PrimaryKey { get; private set; }
        public string GetSQLInternalFields(string prefix = null)
        {
            if (prefix == null)
            {
                prefix = "";
            }
            string rval = "SELECT ";
            for (int i = 0; i < InternalFields.Length; i++)
            {
                if (i > 0) { rval += ", "; }
                rval += prefix.Trim() + InternalFields[i].ColumnName;
            }
            rval += (" FROM " + TableName);

            return rval;
        }
    }
}
