using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Tsukaeru
{
    public static class SqlHelper
    {
        //DataBase Name
        public enum Database { Console };
        private static SqlConnection sqlConnection = null;
        private static SqlConnection OpenDBConnection(string connectionString)
        {
            if (!String.IsNullOrWhiteSpace(connectionString))
            {
                LogHelper.Log(LogHelper.LEVEL.DEBUG, null, "SqlHelper.OpenDBConnection(connectionString = '{0}')", connectionString);
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
                return sqlConnection;
            }
            LogHelper.Log(LogHelper.LEVEL.ERROR, null, "SqlHelper.OpenDBConnection(): connectionString is null");
            return null;
        }
        private static void CloseDBConnection()
        {
            LogHelper.Log(LogHelper.LEVEL.DEBUG, null, "SqlHelper.CloseDBConnection()");
            sqlConnection.Close();
            sqlConnection.Dispose();
        }
        private static SqlCommand CreateCommand(string sqlQuery, Database database)
        {
            string connectionString = ConfigurationManager.AppSettings.Get("DbConnections." + database.ToString());
            sqlConnection = SqlHelper.OpenDBConnection(connectionString);
            if (sqlConnection == null)
            {
                LogHelper.Log(LogHelper.LEVEL.ERROR, null, "SqlHelper.CreateCommand(sqlQuery = '{0}', database = '{1}'): failed to open a connection to the database", sqlQuery, database.ToString());
            }
            LogHelper.Log(LogHelper.LEVEL.DEBUG, null, "SqlHelper.CreateCommand(sqlQuery = '{0}', database = '{1}'): returned command", sqlQuery, database.ToString());
            return new SqlCommand(sqlQuery, sqlConnection);
        }

        public static List<Dictionary<String, Object>> Select(Database database, string selectColumns, string tableName, string where,string limit="10")
        {
            string sqlQuery = string.Format("SELECT top {3} {0} FROM {1} WHERE {2}", selectColumns, tableName, where,limit);

            SqlCommand command = CreateCommand(sqlQuery, database);
            IDataReader dataReader = command.ExecuteReader();

            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            string[] columns = selectColumns.Split(',');
            int rowNumber = 0;
            while (dataReader.Read())
            {
                rows.Add(new Dictionary<string, object>());
                foreach (String column in columns)
                {
                    if (!column.Contains("."))
                    {
                        if (column.ToLower().Contains(" as "))
                        {
                            string column1 = column.Substring(column.LastIndexOf(" as ") + 3);
                            column1 = column1.Trim();
                            rows[rowNumber].Add(column1.Trim(), dataReader[column1.Trim()]);
                        }
                        else
                        {
                            rows[rowNumber].Add(column.Trim(), dataReader[column.Trim()]);
                        }
                    }
                    else
                    {
                        string column1 = column.Substring(column.IndexOf('.') + 1);
                        rows[rowNumber].Add(column1.Trim(), dataReader[column1.Trim()]);
                    }
                }
                rowNumber++;
            }
            dataReader.Close();
            command.Dispose();
            SqlHelper.CloseDBConnection();
            LogHelper.Log(LogHelper.LEVEL.INFO, null, "SqlHelper.Select(): performed query '{0}' and returned '{1}' rows", sqlQuery, rows.Count.ToString());
            return rows;
        }

        public static int ExecuteStoredProc(Database database, string procName, Dictionary<string, string> parameters)
        {
            SqlCommand command = CreateCommand(procName, database);
            command.CommandType = CommandType.StoredProcedure;

            // Add parameters to the command which will be passed to the storedProc
            string paramsString = "";
            foreach (KeyValuePair<string, string> entry in parameters)
            {
                command.Parameters.Add(new SqlParameter(entry.Key, entry.Value));
                paramsString += "[" + entry.Key + "=" + entry.Value + "]";
            }

            IDataReader dataReader = command.ExecuteReader();

            command.Dispose();
            SqlHelper.CloseDBConnection();
            LogHelper.Log(LogHelper.LEVEL.INFO, null, "SqlHelper.ExecuteStoredProc(database = '{0}', procName = '{1}', parameters = '{2}'): affected '{3}' rows", database.ToString(), procName, paramsString, dataReader.RecordsAffected.ToString());
            return dataReader.RecordsAffected;
        }

        public static int ExecuteQuery(string Query, Database database)
        {
            string connectionString = ConfigurationManager.AppSettings.Get("DbConnections." + database.ToString());
            sqlConnection = SqlHelper.OpenDBConnection(connectionString);
            if (sqlConnection == null)
            {
                throw new Exception("SqlHelper failed to open a connection to: '" + connectionString + "'");
            }
            SqlCommand command = new SqlCommand(Query, sqlConnection);
            int rowsAffected = command.ExecuteNonQuery();

            command.Dispose();
            SqlHelper.CloseDBConnection();

            return rowsAffected;
        }
    }
}
