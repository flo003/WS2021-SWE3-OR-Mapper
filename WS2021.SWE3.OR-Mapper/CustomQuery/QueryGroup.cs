using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.CustomQuery
{
    public class QueryGroup
    {
        internal QueryGroup(QueryAction queryAction)
        {
            _queryAction = queryAction;
        }

        private QueryAction _queryAction;
        private ICustomQuery customQueryGroup;
        private CustomQueries customQueries = new CustomQueries();
        private int paramNumber = 1;

        private QueryWhereConnections _nextConnection = QueryWhereConnections.NONE;

        internal void AddCustomQuery(CustomQuery customQuery)
        {
            if(customQueryGroup == null)
            {
                customQueryGroup = new CustomQueryStack() { CustomQueryGroup1 = customQuery };
            }
            else
            {
                if(_nextConnection != QueryWhereConnections.NONE){
                    if(customQueryGroup is CustomQueryStack)
                    {
                        CustomQueryStack group = (customQueryGroup as CustomQueryStack);
                        if(group.CustomQueryGroup1 == null)
                        {
                            group.CustomQueryGroup1 = customQuery;
                        }else if(group.CustomQueryGroup2 == null)
                        {
                            group.CustomQueryGroup2 = customQuery;
                            group.WhereConnections = _nextConnection;
                        }
                        else
                        {
                            CustomQueryStack queryStack = new CustomQueryStack() {
                                CustomQueryGroup1 = group,
                                CustomQueryGroup2 = new CustomQueryStack()
                                {
                                    CustomQueryGroup1 = customQuery,
                                },
                                WhereConnections = _nextConnection
                            };
                            customQueryGroup = queryStack;
                        }
                    }
                }
            }
            _nextConnection = QueryWhereConnections.NONE;
        }

        
        public QueryAction And()
        {
            _nextConnection = QueryWhereConnections.AND;
            return _queryAction;
        }

        public QueryAction Or()
        {
            _nextConnection = QueryWhereConnections.OR;
            return _queryAction;
        }

        internal string GetWhereClause()
        {
            return customQueryGroup.WhereClause;
        }

        internal List<Tuple<string,object>> GetWhereClauseParams()
        {
            return customQueryGroup.WhereClauseParams;
        }

    }
}
