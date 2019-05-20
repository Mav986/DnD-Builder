using System;
using System.IO;
using DnDBuilderLinux.Database;
using NUnit.Framework;

namespace DnDBuilderLinux.Tests
{
    [TestFixture]
    public class TestDatabaseHandler
    {
        private DatabaseHandler _dbHandler;
        
        [SetUp]
        public void SetUp()
        {
            _dbHandler = new DatabaseHandler();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                File.Delete(Schema.Database.Filename);
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
                Assert.That(File.Exists(Schema.Database.Filename));
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
                Assert.True(_dbHandler.TableExists(Schema.Character.Table));
            }
            catch (Exception e)
            {
                Assert.Fail(e + "\n\n" + e.StackTrace);
            }
        }
    }
}