using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WS2021.SWE3.OR_Mapper.ModelEntities;

namespace WS2021.SWE3.OR_Mapper.CustomQuery
{
    internal class CustomQuery : ICustomQuery
    {
        public CustomQuery(QueryWhereActions actions, bool not, ModelField modelField, List<object> arguments, int parameterNumber)
        {
            _not = not;
            _actions = actions;
            _field = modelField;
            _arguments = arguments;
            ParameterNumber = parameterNumber;
            GenerateWhereClause();
        }

        private bool _not;
        private QueryWhereActions _actions;
        private ModelField _field;
        private List<object> _arguments = new List<object>();
        public int ParameterNumber { get; set; } = 1;

        private List<Tuple<string, object>> _parameters;
        private string whereClause = "";

        public string WhereClause { get { return whereClause; } }
        public List<Tuple<string, object>> WhereClauseParams { get { return _parameters; } }

        public void GenerateWhereClause()
        {
            _parameters = new List<Tuple<string, object>>();
            string result = "";
            switch (_actions)
            {
                case QueryWhereActions.EQUALS:
                    result += _not ? " != " : " = ";
                    result += (":para" + ParameterNumber.ToString()) + " ";
                    _parameters.Add(new Tuple<string, object>(":para" + ParameterNumber, _field.ToColumnType(_arguments[0])));
                    ParameterNumber++;
                    break;
                case QueryWhereActions.GT:
                    result += (_not ? " <= " : " > ");
                    result += (":para" + ParameterNumber.ToString()) + " ";
                    _parameters.Add(new Tuple<string, object>(":para" + ParameterNumber, _field.ToColumnType(_arguments[0])));
                    ParameterNumber++;
                    break;
                case QueryWhereActions.LT:
                    result += (_not ? " >= " : " < ");
                    result += (":para" + ParameterNumber.ToString()) + " ";
                    _parameters.Add(new Tuple<string, object>(":para" + ParameterNumber, _field.ToColumnType(_arguments[0])));
                    ParameterNumber++;
                    break;
                case QueryWhereActions.LIKE:
                    result += (_not ? " NOT LIKE " : " LIKE ");
                    result += (":para" + ParameterNumber.ToString()) + " ";
                    _parameters.Add(new Tuple<string, object>(":para" + ParameterNumber, _field.ToColumnType(_arguments[0])));
                    ParameterNumber++;
                    break;
                case QueryWhereActions.IN:
                    result += (_not ? " NOT IN (" : " IN (");
                    string inBrackets = "";
                    for (int i = 0; i < _arguments.Count; i++)
                    {
                        if (i >= 1) { inBrackets += ", "; }
                        inBrackets += (":para" + ParameterNumber.ToString());
                        _parameters.Add(new Tuple<string, object>(":para" + ParameterNumber.ToString(), _field.ToColumnType(_arguments[i])));
                        ParameterNumber++;
                    }
                    result += inBrackets + " ) ";
                    break;
            }
            if (result != "")
            {
                result = " ( " + _field.ColumnName + " " + result + " ) ";
            }
            whereClause = result;
        }
    }
}
