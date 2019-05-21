using System;
using System.Collections.Generic;
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
                    insert = AddParameters(character, insert);
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

        public List<Character> SelectAll()
        {
            try
            {
                List<Character> charList = new List<Character>();
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand cmd = new SqliteCommand(Schema.Character.Query.SelectAll, dbConn);
                    SqliteDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        charList.Add(ConvertToCharacter(reader));
                    }
                }

                return charList;
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        private Character ConvertToCharacter(SqliteDataReader reader)
        {
            return new Character
            {
                Name = (string) reader[Schema.Character.Field.Name],
                Age = (long) reader[Schema.Character.Field.Age],
                Gender = (string) reader[Schema.Character.Field.Gender],
                Biography = (string) reader[Schema.Character.Field.Bio],
                Level = (long) reader[Schema.Character.Field.Level],
                Race = (string) reader[Schema.Character.Field.Race],
                Class = (string) reader[Schema.Character.Field.Class],
                Caster = (bool) reader[Schema.Character.Field.Caster],
                Hitpoints = (long) reader[Schema.Character.Field.Hp],
                Con = (long) reader[Schema.Character.Field.Constitution],
                Dex = (long) reader[Schema.Character.Field.Dexterity],
                Str = (long) reader[Schema.Character.Field.Strength],
                Cha = (long) reader[Schema.Character.Field.Charisma],
                Intel = (long) reader[Schema.Character.Field.Intelligence],
                Wis = (long) reader[Schema.Character.Field.Wisdom]
            };
        }

        private SqliteCommand AddParameters(Character character, SqliteCommand command)
        {
            // All key string's must match key from Schema
            command.Parameters.AddWithValue("@name", character.Name);
            command.Parameters.AddWithValue("@age", character.Age);
            command.Parameters.AddWithValue("@gender", character.Gender);
            command.Parameters.AddWithValue("@bio", character.Biography);
            command.Parameters.AddWithValue("@level", character.Level);
            command.Parameters.AddWithValue("@race", character.Race);
            command.Parameters.AddWithValue("@class", character.Class);
            command.Parameters.AddWithValue("@caster", character.Caster);
            command.Parameters.AddWithValue("@hp", character.Hitpoints);
            command.Parameters.AddWithValue("@con", character.Con);
            command.Parameters.AddWithValue("@dex", character.Dex);
            command.Parameters.AddWithValue("@str", character.Str);
            command.Parameters.AddWithValue("@cha", character.Cha);
            command.Parameters.AddWithValue("@intel", character.Intel);
            command.Parameters.AddWithValue("@wis", character.Wis);

            return command;
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