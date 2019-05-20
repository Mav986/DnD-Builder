using System;
using System.IO;
using DnDBuilderLinux.Database;
using DnDBuilderLinux.Models;
using NUnit.Framework;

namespace DnDBuilderLinux.Tests
{
    [TestFixture]
    public class TestDatabaseHandler
    {
        private const string Name = "Test";
        private DatabaseHandler _dbHandler;
        
        [TestFixtureSetUp]
        public void GlobalSetup()
        {
            _dbHandler = new DatabaseHandler();
        }

        [TestFixtureTearDown]
        public void GlobalTeardown()
        {
            try
            {
                File.Delete(Schema.Database.Filename);
            }
            catch (IOException e)
            {
                Fail(e);
            }
        }
        
        [Test]
        public void CreateDatabase()
        {
            try
            {
                Assert.That(File.Exists(Schema.Database.Filename));
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        [Test]
        public void CreateTable()
        {
            try
            {
                Assert.True(_dbHandler.TableExists(Schema.Character.Table));
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        [Test]
        public void Add()
        {
            try
            {
                Character testChar = new Character {Name = Name};
                _dbHandler.AddCharacter(testChar);

                Assert.Throws<DatabaseException>(() => _dbHandler.AddCharacter(testChar));
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        [Test]
        public void Get()
        {
            Character testChar = new Character {Name = "Hi"};
            _dbHandler.AddCharacter(testChar);
            Assert.DoesNotThrow(() => _dbHandler.GetCharacter(testChar.Name));

            Character compareChar = _dbHandler.GetCharacter(testChar.Name);
            Assert.True(compareChar.Name == testChar.Name);
            
            Assert.IsNull(_dbHandler.GetCharacter("FakeCharacter"));
        }

        private static void Fail(Exception e)
        {
            Assert.Fail(e + "\n\n" + e.StackTrace);
        }
    }
}