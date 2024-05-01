using Microsoft.Identity.Client;
using MySqlConnector;
using StudioBriefcase.Pages.Shared.Components.Posts;
using System.Data;
using System.Text;

namespace StudioBriefcase.Helpers
{
    

    public class QueryHelper
    {
        //public StringBuilder _builder;
        public StringBuilder _selects;
        public StringBuilder _joins;
        public StringBuilder _wheres;
        List<Tuple<string, string>> Parameters;
        List<Tuple<string, uint>> IDParameters;

        public QueryHelper()
        {
            _selects = new StringBuilder();
            _joins = new StringBuilder();
            _wheres = new StringBuilder();
            Parameters = new List<Tuple<string, string>>();
            IDParameters = new List<Tuple<string, uint>>();            
        }
        public void Dispose()
        {          
            _selects.Clear();
            _joins.Clear();
            _wheres.Clear();
            Parameters.Clear();
            IDParameters.Clear();
        }

        public MySqlCommand Build(MySqlConnection connection, MySqlTransaction? transaction = null)
        {
            MySqlCommand command = new MySqlCommand($"{_selects}{_joins}{_wheres};", connection, transaction);
            foreach (Tuple<string, string> param in Parameters)
            {
                command.Parameters.AddWithValue($"@{param.Item1}", param.Item2);
            }
            foreach (Tuple<string,uint> param in IDParameters)
            {
               // Console.WriteLine($"{param.Item1}, {param.Item2}");
                command.Parameters.AddWithValue($"{param.Item1}", param.Item2);
            }
            //Console.WriteLine(command.CommandText);
            Dispose();
            return command;
        }

        public QueryHelper Select(string column)
        {
            _selects.Append($"SELECT {column} ");
            return this;
        }

        public QueryHelper From(string table)
        {
            _selects.Append($"FROM {table} ");
            return this;
        }

        public QueryHelper Join(string query)
        {
            _joins.Append($"JOIN {query} ");
            return this;
        }

        /// <summary>
        /// Empty Where to use with mutliple parameter Entries.
        /// </summary>
        /// <returns></returns>
        public QueryHelper AndPost( uint ID)
        {
            _wheres.Append("AND p.");
            return this;
        }

        //public QueryHelper Where(string column)
        //{
        //    _wheres.Append($"WHERE {column} = @{column} ");
        //    Parameters.Add(column);
        //    return this;
        //}

        public QueryHelper Where(string column, string data)
        {
            _wheres.Append($"WHERE {column} = @{column} ");
            Parameters.Add(new Tuple<string, string>(column, data));
            return this;
        }

        public QueryHelper Where(string column, uint id)
        {
            _wheres.Append($"WHERE {column} = @{column} ");
            IDParameters.Add(new Tuple<string, uint>($"@{column}", id));
            return this;
        }

        public QueryHelper WhereAnd(string column, uint data)
        {
            _wheres.Append($"AND {column} = @{column} ");
            IDParameters.Add(new Tuple<string, uint>($"@{column}", data));
            return this;
        }

        public QueryHelper Order(string column, string direction = "ASC")
        {
            _wheres.Append($"ORDER BY {column} {direction} ");
            return this;
        }
        /// <summary>
        /// Helper Function to retrieve all the Map IDs for Categories, libraries and subjects.
        /// </summary>
        /// <param name="topicID"></param>
        public QueryHelper SelectMapID(uint topicID)
        {
            _selects.Append("SELECT c.id, l.id, s.id FROM topics t ");

            _wheres.Append("WHERE t.id = @topicID ");
            IDParameters.Add(new Tuple<string, uint>("@topicID", topicID));
            return this;
        }

        public QueryHelper JoinMap()
        {
            _joins.Append("JOIN subjects s ON s.id = t.subject_id JOIN libraries l ON l.id = s.library_id JOIN categories c ON c.id = l.category_id ");
            return this;
        }


        public QueryHelper UpdateAlias(string tableName, uint map_id, uint language_id)
        {
            _selects.Append($"UPDATE {tableName}_languages ");
            _wheres.Append($"WHERE map_id = @mapid AND language_id = @langID ");
            IDParameters.Add(new Tuple<string, uint>("@mapid", map_id));
            IDParameters.Add(new Tuple<string, uint>("@langID", language_id));
            return this;
        }

        public QueryHelper SetAlias(string name, string description)
        {
            _joins.Append($"SET alias_name = @alias, alias_description = @description ");
            Parameters.Add(new Tuple<string, string>("alias", name));
            Parameters.Add(new Tuple<string, string>("description", description));
            return this;
        }

        public QueryHelper SetAlias(string name)
        {
            _joins.Append($"SET alias_name = @alias ");
            Parameters.Add(new Tuple<string, string>("alias", name));
            return this;
        }


        public QueryHelper SelectMapName(uint topicID)
        {
            _selects.Append("SELECT c.category_name, l.library_name, s.subject_name, t.topic_name FROM topics t ");
            _wheres.Append("WHERE t.id = @topicID");
            IDParameters.Add(new Tuple<string, uint>("@topicID", topicID));
            return this;
        }
        /// <summary>
        /// Selects the post id, link and alias from the post type table
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public QueryHelper SelectPostRecord(string table)
        {
            _selects.Append($"SELECT pt.id, pt.link, pt.alias FROM post_type_{table} ");
            return this;
        }

        /// <summary>
        /// Initializes a query to select a list of links from a post type table.
        /// </summary>
        /// <param name="postType"></param>
        /// <returns></returns>
        public QueryHelper SelectPostLinks(string table)
        {
            _selects.Append($"SELECT pt.link FROM {table} pt ");
            return this;
        }

        /// <summary>
        /// Joins the Post Table to the Command, and adds the where clause to the command with the supplied parameters.
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="languageID"> Treats Zero as Wildcard by excluding the table from the query.</param>
        /// <param name="section"></param>
        /// <returns></returns>
        public QueryHelper JoinPosts(uint topicID, uint languageID, uint section = 0)
        {
            _joins.Append($"JOIN posts p ON p.id = pt.post_id ");
            _wheres.Append("WHERE p.topics_id = @topicID ");

            IDParameters.Add(new Tuple<string, uint>("@topicID", topicID));
            if (languageID != 0)
            {
                _wheres.Append("AND p.post_language_id = @languageID ");
                IDParameters.Add(new Tuple<string, uint>("@languageID", languageID));
            }
            if (section != 0)
            {
                _wheres.Append("AND p.section = @section ");
                IDParameters.Add(new Tuple<string, uint>("@section", section));
            }


            //if (languageID == 0)
            //{
            //    _wheres.Append("WHERE p.topics_id = @topicID AND p.section = @section ");
            //}
            //else
            //{
            //    _wheres.Append("WHERE p.topics_id = @topicID AND p.post_language_id = @languageID AND p.section = @section ");
            //    IDParameters.Add(new Tuple<string, uint>("@languageID", languageID));
            //}                               
            //IDParameters.Add(new Tuple<string, uint>("@section", section));
            return this;
        }

        public QueryHelper JoinPosts(string category, string library, string subject, string topic, uint languageID, uint section)
        {
            _joins.Append("JOIN posts p ON p.id = pt.post_id JOIN topics t ON t.id = p.topics_id JOIN subjects s on s.id = t.subjects_id JOIN libraries l ON l.id = s.library_id JOIN categories c ON c.id = l.category_id ");
            _wheres.Append("WHERE c.category_name = @category AND l.library_name = @library AND s.subject_name = @subject AND t.topic_name = @topic AND p.post_language_id = @languageID AND p.section = @section ");
            Parameters.Add(new Tuple<string, string>(category, category));
            Parameters.Add(new Tuple<string, string>(library, library));
            Parameters.Add(new Tuple<string, string>(subject, subject));
            Parameters.Add(new Tuple<string, string>(topic, topic));
            IDParameters.Add(new Tuple<string, uint>("@languageID", languageID));
            IDParameters.Add(new Tuple<string, uint>("@section", section));
            return this;
        }

        public QueryHelper JoinTags(List<uint> tags)
        {
            StringBuilder extratags = new StringBuilder();
            if (tags.Count > 0)
            {
                for (int i = 0; i < tags.Count; i++)
                {
                    if (extratags.Length != 0)
                    {
                        extratags.Append($", @tag{i}");
                    }
                    else
                    {
                        extratags.Append($" AND tg.id IN (@tag{i}");
                    }
                    IDParameters.Add(new Tuple<string, uint>($"@tag{i}", tags[i]));    
                }
                extratags.Append(')');

                if(extratags.Length > 0)
                {
                    _joins.Append(" LEFT JOIN tags_posts tp on tp.post_id = p.id LEFT join tags tg on tg.id = tp.tag_id ");
                }
                
                _wheres.Append(extratags);
            }


            return this;
        }


        public QueryHelper InsertPost(uint topicID, uint posttype, uint language_id, uint gitid = 0, uint section = 0)
        {
            _selects.Append("INSERT INTO posts (topics_id, post_type_id, post_language_id, git_id, section) VALUES (@topicID, @posttype, @language_id, @gitid, @section)");
            IDParameters.Add(new Tuple<string, uint>("@topicID", topicID));
            IDParameters.Add(new Tuple<string, uint>("@posttype", posttype));
            IDParameters.Add(new Tuple<string, uint>("@language_id", language_id));
            IDParameters.Add(new Tuple<string, uint>("@gitid", gitid));
            IDParameters.Add(new Tuple<string, uint>("@section", section));
            return this;
        }

        public QueryHelper LastID()
        {
            _wheres.Append("; SELECT LAST_INSERT_ID()");
            return this;
        }



    }



}
