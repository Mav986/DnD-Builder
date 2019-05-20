using System.Data;
using System.IO;
using Mono.Data.Sqlite;

namespace DnDBuilder.Database
{
    public class DatabaseHandler
    {
        private readonly string _filename;
        private readonly string _connectionString;
        
        private SqliteConnection _connection;

        public DatabaseHandler(string filename)
        {
            _filename = filename;
            _connectionString = "Data Source=" + _filename + "; Version=3; Pooling=True;";
            CreateDatabase();
        }

        public void Insert(string sqlCommand)
        {
            
        }

        /// <summary>
        ///     Create the database file on system, if one does not already exists
        /// </summary>
        /// TODO not really sure why it throws an exception...
        /// <exception cref="DatabaseException"></exception>
        private void CreateDatabase()
        {
            try
            {
                if (!File.Exists(_filename))
                {
                    SqliteConnection.CreateFile(_filename);
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
        private void Connect()
        {
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
        }

        private bool ConnectionOpen()
        {
            return _connection.State == ConnectionState.Open;
        }

        /// <summary>
        ///     Close the connection to the database
        /// </summary>
        private void Disconnect()
        {
            _connection.Close();
        }

        public void CreateTable(string createTableCmd)
        {
            try
            {
                using (_connection)
                {
                    Connect();
                    SqliteCommand command = new SqliteCommand(createTableCmd, _connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
            finally
            {
                if (ConnectionOpen())
                {
                    Disconnect();
                }
            }
        }
    }
}