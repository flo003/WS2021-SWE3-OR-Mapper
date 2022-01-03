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
            LocalFields = Fields.Where((field) => field.IsForeignField == false).ToArray();
            ForeignFields = Fields.Where((field) => field.IsForeignField == true).ToArray();
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
            modelField.IsNullable = true; // default nullable
            if (fieldAttribute != null && fieldAttribute is FieldAttribute)
            {
                modelField.ColumnName = fieldAttribute?.ColumnName ?? propertyInfo.Name;
                modelField.ColumnType = propertyInfo.PropertyType;
                modelField.ColumnDbType = fieldAttribute.ColumnDbType;
                modelField.IsNullable = fieldAttribute.Nullable;
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
                modelField.IsNullable = false;
            }

            ForeignKeyAttribute foreignKeyAttribute = modelField.Member.GetCustomAttribute(typeof(ForeignKeyAttribute)) as ForeignKeyAttribute;
            if (foreignKeyAttribute != null && foreignKeyAttribute is ForeignKeyAttribute)
            {
                modelField.IsForeignKey = true;
                modelField.IsForeignField = typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType);
                if (modelField.IsForeignField)
                {
                    modelField.RemoteTableName = foreignKeyAttribute.RemoteTableName;
                    modelField.RemoteTableColumnName = foreignKeyAttribute.RemoteTableColumnName;
                    modelField.IsManyToMany = (!string.IsNullOrWhiteSpace(modelField.RemoteTableName));
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
        public ModelField[] ForeignFields { get; private set; }
        public ModelField[] LocalFields { get; private set; }
        public ModelField PrimaryKey { get; private set; }
        public string GetSQLLocalFields(string prefix = null)
        {
            string resultValue = "SELECT ";
            if (prefix == null)
            {
                prefix = "";
            }
            resultValue += GetSQLLocalFieldsColumns(prefix);
            resultValue += (" FROM " + TableName);

            return resultValue;
        }


        public string GetSQLLocalFieldsColumns(string prefix = "")
        {
            string resultValue = "";
            for (int i = 0; i < LocalFields.Length; i++)
            {
                if (i > 0) { resultValue += ", "; }
                resultValue += prefix.Trim() + LocalFields[i].ColumnName;
            }
            return resultValue;
        }

        public ModelField GetFieldForColumn(string columnName)
        {
            columnName = columnName.ToUpper();
            foreach (ModelField i in LocalFields)
            {
                if (i.ColumnName.ToUpper() == columnName.ToUpper()) { return i; }
            }
            return null;
        }

        public ModelField GetFieldForPropertyInfo(PropertyInfo propertyInfo)
        {
            foreach (ModelField i in LocalFields)
            {
                if (i.Member == propertyInfo) { return i; }
            }
            return null;
        }
    }
}
