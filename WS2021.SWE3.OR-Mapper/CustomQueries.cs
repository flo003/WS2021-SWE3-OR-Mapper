using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper
{
    internal class CustomQueries
    {
        private IDbCommand command;

        public CustomQueries(IDbConnection dbConnection)
        {
            command = dbConnection.CreateCommand();

            command.CommandText += "SELECT";
        }

        public CustomQueries Select(ModelEntity modelEntities)
        {
            command.CommandText += modelEntities.GetSQLLocalFieldsColumns();
            return this;
        }

        public IDbCommand Setup()
        {
            return command;
        }
    }
}
