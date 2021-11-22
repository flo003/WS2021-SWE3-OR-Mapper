using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper.CustomQuery
{
    internal class CustomQueries
    {
        

        public ICustomQuery And(CustomQuery customQuery1, CustomQuery customQuery2)
        {
            return new CustomQueryGroup(customQuery1, customQuery2, QueryWhereConnections.AND);
        }

        public ICustomQuery Or(CustomQuery customQuery1, CustomQuery customQuery2)
        {
            return new CustomQueryGroup(customQuery1, customQuery2, QueryWhereConnections.OR);
        }

        public ICustomQuery And(ICustomQuery customQueryGroup1, ICustomQuery customQueryGroup2)
        {
            return new CustomQueryStack(customQueryGroup1, customQueryGroup2, QueryWhereConnections.AND);
        }

        public ICustomQuery Or(ICustomQuery customQueryGroup1, ICustomQuery customQueryGroup2)
        {
            return new CustomQueryStack(customQueryGroup1, customQueryGroup2, QueryWhereConnections.OR);
        }

        public CustomQuery Equals(bool not, ModelField field, object value, int paramNumber = 1)
        {
            CustomQuery customQuery = new CustomQuery(QueryWhereActions.EQUALS, not, field, new() { value }, paramNumber);
            return customQuery;
        }

        public CustomQuery GreaterThan(bool not, ModelField field, object value, int paramNumber = 1)
        {
            CustomQuery customQuery = new CustomQuery(QueryWhereActions.GT, not, field, new() { value }, paramNumber);
            return customQuery;
        }

        public CustomQuery LesserThan(bool not, ModelField field, object value, int paramNumber = 1)
        {
            CustomQuery customQuery = new CustomQuery(QueryWhereActions.LT, not, field, new() { value }, paramNumber);
            return customQuery;
        }

        public CustomQuery Like(bool not, ModelField field, object value, int paramNumber = 1)
        {
            CustomQuery customQuery = new CustomQuery(QueryWhereActions.LIKE, not, field, new() { value }, paramNumber);
            return customQuery;
        }

        public CustomQuery In(bool not, ModelField field, List<object> values, int paramNumber = 1)
        {
            CustomQuery customQuery = new CustomQuery(QueryWhereActions.IN, not, field, values, paramNumber);
            return customQuery;
        }
        
    }
}
