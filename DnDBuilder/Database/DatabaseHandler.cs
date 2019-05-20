using System;
using System.IO;
using Mono.Data.Sqlite;

namespace DnDBuilder.Database
{
    public class DatabaseHandler
    {
        private readonly string _filename;

        public DatabaseHandler(string filename)
        {
            this._filename = filename;
        }

        private void CreateDatabase()
        {
            try
            {
                if (!File.Exists(this._filename))
                {
                    SqliteConnection.CreateFile(this._filename);
                }
            }
            catch (SqliteException e)
            {
                throw new InvalidFilenameException(e.Message, e);
            }
        }
    }
}