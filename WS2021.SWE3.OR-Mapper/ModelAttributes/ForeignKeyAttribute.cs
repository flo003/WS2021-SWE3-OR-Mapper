using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.ModelAttributes
{
    public class ForeignKeyAttribute : FieldAttribute
    {
        public string RemoteTableName = null;

        public string RemoteTableColumnName = null;
    }
}
