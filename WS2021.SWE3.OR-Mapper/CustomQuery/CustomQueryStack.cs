using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.CustomQuery
{
    internal class CustomQueryStack : ICustomQuery
    {
        private ICustomQuery _customQueryGroup1;
        private ICustomQuery _customQueryGroup2;
        private QueryWhereConnections _whereConnections;
        internal ICustomQuery CustomQueryGroup1
        {
            get { return _customQueryGroup1; }
            set { _customQueryGroup1 = value; }
        }

        internal ICustomQuery CustomQueryGroup2
        {
            get { return _customQueryGroup2; }
            set { _customQueryGroup2 = value; }
        }
        internal QueryWhereConnections WhereConnections
        {
            get { return _whereConnections; }
            set { _whereConnections = value; }
        }
        public CustomQueryStack() { }
        public CustomQueryStack(ICustomQuery customQueryGroup1, ICustomQuery customQueryGroup2 = null, QueryWhereConnections whereConnections = QueryWhereConnections.AND)
        {
            _customQueryGroup1 = customQueryGroup1;
            _customQueryGroup2 = customQueryGroup2;
            _whereConnections = whereConnections;
        }
        public List<Tuple<string, object>> WhereClauseParams
        {
            get
            {
                List<Tuple<string, object>> result = new();
                if (_customQueryGroup1 != null)
                {
                    result.AddRange(_customQueryGroup1.WhereClauseParams);
                }
                if (_customQueryGroup2 != null)
                {
                    result.AddRange(_customQueryGroup2.WhereClauseParams);
                }
                return result;
            }
        }

        public string WhereClause
        {
            get
            {
                string where1 = "";
                if (_customQueryGroup1 != null)
                {
                    where1 = _customQueryGroup1.WhereClause;
                }
                string where2 = "";
                if (_customQueryGroup2 != null)
                {
                    where2 = _customQueryGroup2.WhereClause;
                }
                if (where1 != "" && where2 != "")
                {
                    return $" ( {where1} {parseQuery_whereConnections(_whereConnections)} {where2} ) ";
                }
                else if (where1 != "")
                {
                    return $" {where1} ";
                }
                else if (where2 != "")
                {
                    return $" {where2} ";
                }
                return " ";
            }
        }

        private string parseQuery_whereConnections(QueryWhereConnections _whereConnections)
        {
            string result = " AND ";
            switch (_whereConnections)
            {
                case QueryWhereConnections.AND:
                    result = " AND ";
                    break;
                case QueryWhereConnections.OR:
                    result = " OR ";
                    break;
            }
            return result;
        }
    }
}
