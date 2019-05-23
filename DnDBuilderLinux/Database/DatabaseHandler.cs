using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

        /// <summary>
        ///     Insert a character into the database
        /// </summary>
        /// <param name="character">Character to be inserted</param>
        /// <exception cref="DatabaseException"></exception>
        public void InsertCharacter(Character character)
        {
            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    if (CheckExists(character.Name)) throw new DatabaseException("Character already exists");
                    SqliteCommand insert = new SqliteCommand(Schema.Character.Query.InsertCharacter, dbConn);
                    insert = AddCharacterParams(character, insert);
                    insert.ExecuteNonQuery();
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>
        ///     Select all characters from the database
        /// </summary>
        /// <returns>An enumerable containing all Characters</returns>
        /// <exception cref="DatabaseException"></exception>
        public IEnumerable<Character> SelectAllCharacters(Func<IDataRecord, Character> sanitizeCallback)
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
                        charList.Add(sanitizeCallback(reader));
                    }
                }

                return charList;
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>
        ///     Select a character from the database
        /// </summary>
        /// <param name="name">Name of the character to select</param>
        /// <param name="sanitizeCharacter">Callback to sanitize character fields before creating an object</param>
        /// <returns>The selected Character object</returns>
        /// <exception cref="DatabaseException"></exception>
        public Character SelectCharacter(string name, Func<IDataRecord, Character> sanitizeCharacter)
        {
            
            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand select = new SqliteCommand(Schema.Character.Query.SelectCharacter, dbConn);
                    select.Parameters.AddWithValue("@" + Schema.Character.Field.Name, name);
                    SqliteDataReader reader = select.ExecuteReader();
                    if(!reader.Read()) throw new DatabaseException("Character " + name + " not found");
                    return sanitizeCharacter(reader);
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>
        ///     Update a single character's details
        /// </summary>
        /// <param name="name">Name of the character to update</param>
        /// <param name="updatedFields">A dictionary of fields to be updated and their values</param>
        /// <exception cref="DatabaseException"></exception>
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
        ///     Delete a single character
        /// </summary>
        /// <param name="name">Name of the character to be deleted</param>
        /// <exception cref="DatabaseException"></exception>
        public void DeleteCharacter(string name)
        {
            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand cmd = new SqliteCommand(Schema.Character.Query.DeleteCharacter, dbConn);
                    cmd.Parameters.AddWithValue("@" + Schema.Character.Field.Name, name);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }

        /// <summary>
        ///     Open a connection to the database
        /// </summary>
        private static SqliteConnection GetConnection()
        {
            SqliteConnection connection = new SqliteConnection(Schema.Database.Connect);
            connection.Open();
            
            return connection;
        }

        /// <summary>
        ///     Generate a SQL "SET" string from a dictionary of column names and values
        /// </summary>
        /// <param name="dict">A dictionary containing a column name (key) and a value</param>
        /// <returns>A parameter-ready string to be inserted after the SQL "SET" keyword</returns>
        private static string GenerateSqlSetString(Dictionary<string, string> dict)
        {
            string setString = "";
            
            foreach (KeyValuePair<string, string> entry in dict)
            {
                string columnName = SanitizeColumn(entry.Key);
                setString += columnName + "=@" + columnName + ", ";
            }

            return setString.Substring(0, setString.Length - 2);
        }

        /// <summary>
        ///     Sanitize a column name to prevent sql injection
        /// </summary>
        /// <param name="column">name of column to sanitize</param>
        /// <returns></returns>
        /// <exception cref="DatabaseException"></exception>
        private static string SanitizeColumn(string column)
        {
            /*
             * Find the first constant column name that matches the string 'column'
             * This will prevent SQL injection by throwing an exception when the columnName
             * contains anything other than the name of a column.
             */
            string newColumnName = Schema.Character.Field.AllFields.First(x => x.Equals(column, StringComparison.OrdinalIgnoreCase));

            if (newColumnName == null) throw new DatabaseException("Column " + column + " not found");

            return newColumnName;
        }

        /// <summary>
        ///     Add all character fields as parameters
        /// </summary>
        /// <param name="character">Character to provide parameters</param>
        /// <param name="command">Command to add parameters to</param>
        /// <returns>A fully parameterized SqliteCommand</returns>
        /// <exception cref="DatabaseException"></exception>
        private static SqliteCommand AddCharacterParams(Character character, SqliteCommand command)
        {
            try
            {
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Name, character.Name);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Age, character.Age);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Gender, character.Gender);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Bio, character.Biography);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Level, character.Level);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Race, character.Race);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Class, character.Class);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Constitution, character.Con);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Dexterity, character.Dex);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Strength, character.Str);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Charisma, character.Cha);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Intelligence, character.Intel);
                command.Parameters.AddWithValue("@" + Schema.Character.Field.Wisdom, character.Wis);
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }

            return command;
        }

        /// <summary>
        ///     Create the database file on system, if one does not already exists
        /// </summary>
        /// <exception cref="DatabaseException"></exception>
        private static void CreateDatabase()
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
        private static void CreateCharacterTable()
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
        ///     Check if a character already exists in the database
        /// </summary>
        /// <param name="characterName">Name of character to check</param>
        /// <returns></returns>
        /// <exception cref="DatabaseException"></exception>
        private static bool CheckExists(string characterName)
        {
            try
            {
                using (SqliteConnection dbConn = GetConnection())
                {
                    SqliteCommand checkDuplicates = new SqliteCommand(Schema.Character.Query.FindCharacter, dbConn);
                    checkDuplicates.Parameters.AddWithValue("@" + Schema.Character.Field.Name, characterName);
                    int count = Convert.ToInt32(checkDuplicates.ExecuteScalar());
                
                    return count > 0;
                }
            }
            catch (SqliteException e)
            {
                throw new DatabaseException(e.Message, e);
            }
        }
    }
}