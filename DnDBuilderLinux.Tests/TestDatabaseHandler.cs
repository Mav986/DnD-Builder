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
                Hitpoints = 42,
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
                Biography = "",
                Level = 19,
                Race = "elf",
                Class = "paladin",
                Hitpoints = 1,
                Con = 6,
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
        public void AddCharacter()
        {
            try
            {
                Assert.DoesNotThrow(() => _dbHandler.AddCharacter(_testCharOne));
                Assert.Throws<DatabaseException>(() => _dbHandler.AddCharacter(_testCharOne));
                Assert.DoesNotThrow(() => _dbHandler.AddCharacter(_testCharTwo));
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
                Assert.DoesNotThrow(() => compareChar = _dbHandler.GetCharacter(_testCharOne.Name));
                Assert.True(_testCharOne.Name == compareChar.Name);

                Assert.DoesNotThrow(() => compareChar = _dbHandler.GetCharacter(_testCharTwo.Name));
                Assert.True(compareChar.Name == _testCharTwo.Name);

                Assert.IsNull(_dbHandler.GetCharacter("FakeCharacter"));
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