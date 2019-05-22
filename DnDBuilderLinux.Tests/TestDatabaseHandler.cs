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
        private Character _testCharOne;
        private Character _testCharTwo;
        private DatabaseHandler _dbHandler;
        
        [TestFixtureSetUp]
        public void GlobalSetup()
        {
            _dbHandler = new DatabaseHandler();
            
            _testCharOne = new Character
            {
                Name = "testCharOne",
                Age = 499,
                Gender = "Your mom's house",
                Biography = "Lorem Ipsum Here",
                Level = 2,
                Race = "human",
                Class = "barbarian",
                Con = 1,
                Dex = 2,
                Str = 3,
                Cha = 4,
                Intel = 5,
                Wis = 6
            };
            
            _testCharTwo = new Character
            {
                Name = "testCharTwo",
                Age = 1,
                Gender = "",
                Level = 19,
                Race = "elf",
                Class = "paladin",
                Dex = 5,
                Str = 4,
                Cha = 3,
                Intel = 2,
                Wis = 1
            };
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
        public void AddCharacter()
        {
            try
            {
                Assert.DoesNotThrow(() => _dbHandler.InsertCharacter(_testCharOne));
                Assert.Throws<DatabaseException>(() => _dbHandler.InsertCharacter(_testCharOne));
                Assert.DoesNotThrow(() => _dbHandler.InsertCharacter(_testCharTwo));
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        [Test]
        public void GetCharacter()
        {
            try
            {
                Character compareChar = null;
                Assert.DoesNotThrow(() => compareChar = _dbHandler.SelectCharacter(_testCharOne.Name));
                Assert.True(_testCharOne.Name == compareChar.Name);
                Console.WriteLine(_testCharOne.ToString());

                Assert.DoesNotThrow(() => compareChar = _dbHandler.SelectCharacter(_testCharTwo.Name));
                Assert.True(compareChar.Name == _testCharTwo.Name);
                Console.WriteLine(_testCharTwo.ToString());

                Assert.IsNull(_dbHandler.SelectCharacter("FakeCharacter"));
            }
            catch (Exception e)
            {
                Fail(e);
            }
        }

        private static void Fail(Exception e)
        {
            Assert.Fail(e + "\n\n" + e.StackTrace);
        }
    }
}