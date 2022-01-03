using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper.CustomQuery
{
    internal class CustomQueryGroup : ICustomQuery
    {
        internal CustomQueryGroup() { }
        internal CustomQueryGroup(CustomQuery customQuery1, CustomQuery customQuery2, QueryWhereConnections whereConnections)
        {
            _customQuery1 = customQuery1;
            _customQuery2 = customQuery2;
            _whereConnections = whereConnections;
        }

        internal CustomQuery CustomQuery1 {
            get { return _customQuery1; }
            set { _customQuery1 = value; } 
        }

        internal CustomQuery CustomQuery2
        {
            get { return _customQuery2; }
            set { _customQuery2 = value; }
        }
        internal QueryWhereConnections WhereConnections
        {
            get { return _whereConnections; }
            set { _whereConnections = value; }
        }

        private CustomQuery _customQuery1;
        private CustomQuery _customQuery2;
        private QueryWhereConnections _whereConnections;

        public List<Tuple<string, object>> WhereClauseParams 
        { 
            get {
                List<Tuple<string, object>> result = new();
                if(_customQuery1 != null)
                {
                    result.AddRange(_customQuery1.WhereClauseParams);
                }
                if (_customQuery2 != null)
                {
                    result.AddRange(_customQuery2.WhereClauseParams);
                }
                return result;
            } }

        public string WhereClause
        {
            get
            {
                string where1 = _customQuery1.WhereClause;
                string where2 = _customQuery2.WhereClause;
                if (where1 != "" && where2 != "")
                {
                    return $" ( {where1} {parseQueryWhereConnections(_whereConnections)} {where2} ) ";
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

        private string parseQueryWhereConnections(QueryWhereConnections _whereConnections)
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
