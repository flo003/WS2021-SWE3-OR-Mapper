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
    public class QueryAction<K>
    {
        public QueryAction(EntityRegistry entityRegistry)
        {
            _entityRegistry = entityRegistry;
            _queryGroup = new QueryGroup<K>(this);
        }

        private EntityRegistry _entityRegistry;
        private CustomQueries customQueries = new CustomQueries();
        private int paramNumber = 1;
        private QueryGroup<K> _queryGroup;

        private object ParseValueFromExpression(Expression body)
        {
            var mex = body as MemberExpression;
            var fex = mex.Expression as MemberExpression;
            var cex = fex.Expression as ConstantExpression;
            var fld = fex.Member as FieldInfo;
            return fld.GetValue(cex.Value);
        }

        public QueryGroup<K> Equals<T>(Expression<Func<K, T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.Equals(false, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup<K> NotEquals<T>(Expression<Func<K, T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.Equals(true, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup<K> GreaterThan<T>(Expression<Func<K, T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.GreaterThan(false, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }
        public QueryGroup<K> LessOrEqual<T>(Expression<Func<K, T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.GreaterThan(true, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup<K> LesserThan<T>(Expression<Func<K, T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.LesserThan(false, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup<K> GreaterOrEqualThan<T>(Expression<Func<K, T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.LesserThan(true, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup<K> Like<T>(Expression<Func<K, T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.Like(false, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup<K> NotLike<T>(Expression<Func<K, T>> property, T value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.Like(true, field, value, paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber++;
            return _queryGroup;
        }

        public QueryGroup<K> In<T>(Expression<Func<K, T>> property, List<T> value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.In(false, field, value.Select((x) => x as object).ToList(), paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber = customQuery.ParameterNumber + 1;
            return _queryGroup;
        }

        public QueryGroup<K> NotIn<T>(Expression<Func<K, T>> property, List<T> value)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            ModelEntity modelEntity = _entityRegistry.GetModelEntity(typeof(K));
            ModelField field = modelEntity.GetFieldForPropertyInfo(propertyInfo);
            CustomQuery customQuery = customQueries.In(true, field, value.Select((x) => x as object).ToList(), paramNumber);
            _queryGroup.AddCustomQuery(customQuery);
            paramNumber = customQuery.ParameterNumber + 1;
            return _queryGroup;
        }

    }
}
