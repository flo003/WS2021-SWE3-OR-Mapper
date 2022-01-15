using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.Exceptions
{
    internal class DbTypeConversionNotFound : RepositoryException
    {
        public DbTypeConversionNotFound(string message)
            : base(message)
        {

        }

        public DbTypeConversionNotFound(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
