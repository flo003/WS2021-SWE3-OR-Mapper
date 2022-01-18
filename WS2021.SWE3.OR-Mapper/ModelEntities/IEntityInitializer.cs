using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.ModelEntities
{
    internal interface IEntityInitializer
    {
        public object InitEntity(Type type, object primaryKey);
    }
}
