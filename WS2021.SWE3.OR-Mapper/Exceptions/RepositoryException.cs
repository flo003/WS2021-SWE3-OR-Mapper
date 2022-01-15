using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS2021.SWE3.OR_Mapper.Exceptions
{
    internal class RepositoryException : Exception
    {
        public RepositoryException(string message)
            : base(message)
        {

        }

        public RepositoryException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
