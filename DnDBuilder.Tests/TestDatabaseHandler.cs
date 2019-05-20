using DnDBuilder.Database;
using NUnit.Framework;

namespace DnDBuilder.Tests
{
    [TestFixture]
    public class TestDatabaseHandler
    {
        private string _filename;
        private DatabaseHandler _dbHandler;
        
        [SetUp]
        public void SetUp()
        {
            _filename = "mydb.sqlite";
            _dbHandler = new DatabaseHandler(_filename);
        }
        
        [Test]
        public void Creation()
        {
            Assert.That(_filename, Does.Exist);
        }

        [Test]
        public void TableCreation()
        {
            string sql = "create table if not exists " + DndSchema.Character.Table + @" (
                @Name text primary key
            )";
            _dbHandler.CreateTable(sql);
            Assert.That(() => _dbHandler.CreateTable(sql), Throws.Nothing);
        }
    }
}