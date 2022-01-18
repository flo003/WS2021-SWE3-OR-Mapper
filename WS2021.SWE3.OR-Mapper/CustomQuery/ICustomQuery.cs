using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.CustomQuery
{
    public interface ICustomQuery
    {
        public List<Tuple<string, object>> WhereClauseParams { get; }
        public string WhereClause { get; }
       // public void AddCustomQuery(CustomQuery customQuery);
    }
}
