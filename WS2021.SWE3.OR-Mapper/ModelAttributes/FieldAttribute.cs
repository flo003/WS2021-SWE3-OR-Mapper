using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.ModelAttributes
{
    public class FieldAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public DbType ColumnDbType { get; set; }
        public bool Nullable { get; set; } = true;
    }
}
