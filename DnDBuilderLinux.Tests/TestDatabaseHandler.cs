using System;
using System.IO;
using DnDBuilderLinux.Database;
using NUnit.Framework;

namespace DnDBuilderLinux.Tests
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

        [TearDown]
        public void TearDown()
        {
            try
            {
                File.Delete(_filename);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }
        
        [Test]
        public void Creation()
        {
            try
            {
                Assert.That(File.Exists(_filename));
            }
            catch (Exception e)
            {
                Assert.Fail(e + "\n\n" + e.StackTrace);
            }
        }

        [Test]
        public void TableCreation()
        {
            try
            {
                _dbHandler.CreateTable(DndSchema.Character.CreateTableQuery);
                Assert.DoesNotThrow(() => _dbHandler.CreateTable(DndSchema.Character.CreateTableQuery));
                Assert.True(_dbHandler.TableExists(DndSchema.Character.Table));
            }
            catch (Exception e)
            {
                Assert.Fail(e + "\n\n" + e.StackTrace);
            }
        }
    }
}