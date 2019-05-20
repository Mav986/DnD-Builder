using System;
using System.IO;
using DnDBuilderLinux.Models;
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

        public void AddCharacter(Character character)
        {
            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand checkDuplicates = new SqliteCommand(Schema.Character.Query.FindCharacter, dbConn);
                    checkDuplicates.Parameters.AddWithValue(Schema.Param.Name, character.Name);
                    
                    int count = Convert.ToInt32(checkDuplicates.ExecuteScalar());
                    if (count > 0) throw new DatabaseException("Character already exists");

                    SqliteCommand insert = new SqliteCommand(Schema.Character.Query.InsertCharacter, dbConn);
                    insert.Parameters.AddWithValue(Schema.Param.Name, character.Name);
                    insert.ExecuteNonQuery();
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        public Character GetCharacter(string name)
        {
            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand select = new SqliteCommand(Schema.Character.Query.SelectCharacter, dbConn);
                    select.Parameters.AddWithValue(Schema.Param.Name, name);
                    SqliteDataReader reader = select.ExecuteReader();
                    if (reader.Read())
                    {
                        return new Character
                        {
                            Name = reader.GetString(0)
                        };
                    }
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
            
            return null;
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
                    cmd.Parameters.AddWithValue(Schema.Param.Name, name);
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
        private static SqliteConnection GetConnection()
        {
            SqliteConnection connection = new SqliteConnection(Schema.Database.Connect);
            connection.Open();
            
            return connection;
        }
    }
}