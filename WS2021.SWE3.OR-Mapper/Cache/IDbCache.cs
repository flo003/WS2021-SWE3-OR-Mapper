using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.Cache
{
    public interface IDbCache
    {
        public void StoreValue(object value);
        public void RemoveValue(object value);
        public object GetValue(Type type, object primaryKey);
    }
}
