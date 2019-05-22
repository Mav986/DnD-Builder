using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DnDBuilderLinux.Models;
using Mono.Data.Sqlite;

namespace DnDBuilderLinux.Database
{
    // TODO Move all query strings into their functions
    public class DatabaseHandler
    {
        public DatabaseHandler()
        {
            CreateDatabase();
            CreateCharacterTable();
        }

        public void InsertCharacter(Character character)
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

        public Character SelectCharacter(string name)
        {
            Character selectedChar;
            
            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand select = new SqliteCommand(Schema.Character.Query.SelectCharacter, dbConn);
                    select.Parameters.AddWithValue(Schema.Param.Name, name);
                    SqliteDataReader reader = select.ExecuteReader();
                    if(!reader.Read()) throw new DatabaseException("Character " + name + " not found");
                    selectedChar = ConvertToCharacter(reader);
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }

            return selectedChar;
        }

        public IEnumerable<Character> SelectAllCharacters()
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

        public void UpdateCharacter(string name, Dictionary<string, string> updatedFields)
        {
            try
            {
                string fieldsEqualTo = GenerateSqlSetString(updatedFields);
                
                string updateQuery = "UPDATE " + Schema.Character.Table + " " +
                                     "SET " + fieldsEqualTo + " " +
                                     "WHERE " + Schema.Character.Field.Name + "=@name";
                
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand sql = new SqliteCommand(updateQuery, dbConn);
                    foreach (KeyValuePair<string, string> entry in updatedFields)
                    {
                        sql.Parameters.AddWithValue("@" + SanitizeColumn(entry.Key), entry.Value);
                    }
                    sql.Parameters.AddWithValue("@" + Schema.Character.Field.Name, name);

                    sql.ExecuteNonQuery();
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>
        ///     Generate a SQL "SET" string from a dictionary of column names and values
        /// </summary>
        /// <param name="dict">A dictionary containing a column name (key) and a value</param>
        /// <returns>A parameter-ready string to be inserted after the SQL "SET" keyword</returns>
        private string GenerateSqlSetString(Dictionary<string, string> dict)
        {
            string setString = "";
            
            foreach (KeyValuePair<string, string> entry in dict)
            {
                string columnName = SanitizeColumn(entry.Key);
                setString += columnName + "=@" + columnName + ", ";
            }

            return setString.Substring(0, setString.Length - 2);
        }

        private string SanitizeColumn(string column)
        {
            string columnName;
            
            switch (column)
            {
                case Schema.Character.Field.Age: 
                    columnName = Schema.Character.Field.Age;
                    break;
                
                case Schema.Character.Field.Gender: 
                    columnName = Schema.Character.Field.Gender;
                    break;
                
                case Schema.Character.Field.Bio: 
                    columnName = Schema.Character.Field.Bio;
                    break;
                
                case Schema.Character.Field.Level: 
                    columnName = Schema.Character.Field.Level;
                    break;
                
                case Schema.Character.Field.Race: 
                    columnName = Schema.Character.Field.Race;
                    break;
                
                case Schema.Character.Field.Class: 
                    columnName = Schema.Character.Field.Class;
                    break;
                
                case Schema.Character.Field.Constitution: 
                    columnName = Schema.Character.Field.Constitution;
                    break;
                
                case Schema.Character.Field.Dexterity:
                    columnName = Schema.Character.Field.Dexterity;
                    break;
                
                case Schema.Character.Field.Strength: 
                    columnName = Schema.Character.Field.Strength;
                    break;
                
                case Schema.Character.Field.Charisma: 
                    columnName = Schema.Character.Field.Charisma;
                    break;
                
                case Schema.Character.Field.Intelligence: 
                    columnName = Schema.Character.Field.Intelligence;
                    break;
                
                case Schema.Character.Field.Wisdom: 
                    columnName = Schema.Character.Field.Wisdom;
                    break;
                
                default:
                    throw new DatabaseException($"Field \"{column}\" not found");
            }

            return columnName;
        }

        /// <summary>
        ///     Convert database reader results into a character object
        /// </summary>
        /// <param name="reader">A database reader holding results</param>
        /// <returns>A character object</returns>
        private Character ConvertToCharacter(IDataRecord reader)
        {
            return new Character
            {
                Name = reader[Schema.Character.Field.Name] as string,
                Age = reader[Schema.Character.Field.Age] as long? ?? 0,
                Gender = reader[Schema.Character.Field.Gender] as string,
                Biography = reader[Schema.Character.Field.Bio] as string,
                Level = reader[Schema.Character.Field.Level] as long? ?? 0,
                Race = reader[Schema.Character.Field.Race] as string,
                Class = reader[Schema.Character.Field.Class] as string,
                Con = reader[Schema.Character.Field.Constitution] as long? ?? 0,
                Dex = reader[Schema.Character.Field.Dexterity] as long? ?? 0,
                Str = reader[Schema.Character.Field.Strength] as long? ?? 0,
                Cha = reader[Schema.Character.Field.Charisma] as long? ?? 0,
                Intel = reader[Schema.Character.Field.Intelligence] as long? ?? 0,
                Wis = reader[Schema.Character.Field.Wisdom] as long? ?? 0
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