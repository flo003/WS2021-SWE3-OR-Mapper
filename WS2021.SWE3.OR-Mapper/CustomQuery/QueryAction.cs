using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper.CustomQuery
{
    public class QueryAction
    {
        public QueryAction(EntityRegistry entityRegistry)
        {
            _entityRegistry = entityRegistry;
            _queryGroup = new QueryGroup(this);
        }

        private EntityRegistry _entityRegistry;
        private CustomQueries customQueries = new CustomQueries();
        private int paramNumber = 1;
        private QueryGroup _queryGroup;

        private object ParseValueFromExpression(Expression body)
        {
            var mex = body as MemberExpression;
            var fex = mex.Expression as MemberExpression;
            var cex = fex.Expression as ConstantExpression;
            var fld = fex.Member as FieldInfo;
            return fld.GetValue(cex.Value);
        }

        public QueryGroup Equals<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.Equals(false, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup NotEquals<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.Equals(true, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup GreaterThan<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.GreaterThan(false, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }
        public QueryGroup LessOrEqual<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.GreaterThan(true, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup LesserThan<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.LesserThan(false, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup GreaterOrEqualThan<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.LesserThan(true, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup Like<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.Like(false, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup NotLike<T>(Expression<Func<T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.Like(true, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup In<T>(Expression<Func<T>> property, List<T> value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.In(false, field, value.Select((x) => x as object).ToList(), paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber = customQuery.ParameterNumber + 1;
            return _queryGroup;
        }

        public QueryGroup NotIn<T>(Expression<Func<T>> property, List<T> value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            var valueT = ParseValueFromExpression(property.Body);
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(valueT.GetType());
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            var valueProp = field.GetValue(valueT);
            CustomQuery customQuery = customQueries.In(true, field, value.Select((x) => x as object).ToList(), paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber = customQuery.ParameterNumber + 1;
            return _queryGroup;
        }

    }
}
