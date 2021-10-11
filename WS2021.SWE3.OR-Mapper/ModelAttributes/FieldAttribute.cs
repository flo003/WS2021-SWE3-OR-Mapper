using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.ModelAttributes
{
    public class FieldAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public string ColumnTypeName { get; set; }
    }
}
