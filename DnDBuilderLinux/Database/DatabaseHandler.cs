using System.IO;
using Mono.Data.Sqlite;

namespace DnDBuilderLinux.Database
{
    public class DatabaseHandler
    {
        public DatabaseHandler()
        {
            CreateDatabase();
            CreateCharacterTable();
        }

        /// <summary>
        ///     Check if a specific table exists in the database
        /// </summary>
        /// <param name="name">The name of the table</param>
        /// <returns>True if the table exists, false otherwise</returns>
        public bool TableExists(string name)
        {
            bool exists;

            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand cmd = new SqliteCommand(Schema.Character.Query.FindTable, dbConn);
                    cmd.Parameters.AddWithValue(Schema.Character.Parameter.Name, name);
                    SqliteDataReader reader = cmd.ExecuteReader();
                
                    exists = reader.HasRows;
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }

            return exists;
        }

        /// <summary>
        ///     Create the database file on system, if one does not already exists
        /// </summary>
        /// <exception cref="DatabaseException"></exception>
        private void CreateDatabase()
        {
            try
            {
                if (!File.Exists(Schema.Database.Filename))
                {
                    SqliteConnection.CreateFile(Schema.Database.Filename);
                }
            }
            catch (IOException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>
        ///     Create a table to store character data
        /// </summary>
        /// <exception cref="DatabaseException"></exception>
        private void CreateCharacterTable()
        {
            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand cmd = new SqliteCommand(Schema.Character.Query.CreateTable, dbConn);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>
        ///     Open the connection to the database
        /// </summary>
        private SqliteConnection GetConnection()
        {
            SqliteConnection connection = new SqliteConnection(Schema.Database.Query.Connect);
            connection.Open();

            return connection;
        }
    }
}