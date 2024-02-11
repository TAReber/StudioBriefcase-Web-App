using MySqlConnector;
using System.Text;

namespace StudioBriefcase.Helpers
{
    

    public class QueryHelper
    {
        public StringBuilder _builder;

        public QueryHelper()
        {

            _builder = new StringBuilder();
        }

        public MySqlCommand Build(MySqlConnection connection, MySqlTransaction? transaction = null)
        {
            return new MySqlCommand(_builder.Append(';').ToString(), connection, transaction);
        }

        public QueryHelper Select(string column)
        {
            _builder.Append($"SELECT {column} ");
            return this;
        }

        public QueryHelper From(string table)
        {
            _builder.Append($"FROM {table} ");
            return this;
        }

        public QueryHelper Where(string column)
        {
            _builder.Append($"WHERE {column} = @{column} ");
            return this;
        }

        public QueryHelper Order(string column, string direction = "ASC")
        {
            _builder.Append($"ORDER BY {column} {direction} ");
            return this;
        }
    }



}
