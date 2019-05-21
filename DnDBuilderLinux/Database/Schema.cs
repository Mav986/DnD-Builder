namespace DnDBuilderLinux.Database
{
    public static class Schema
    {
        public static class Database
        {
            public const string Filename = "dndbuilder.sqlite";
            public const string Connect = "Data Source=" + Filename + "; Version=3; Pooling=True;";
        }
        public static class Character
        {
            public const string Table = "characters";

            public static class Query
            {
                public const string CreateTable = "CREATE TABLE IF NOT EXISTS " +
                                                   Table + "(" +
                                                   Field.Name + " varchar(20) primary key, " +
                                                   Field.Age + " integer, " +
                                                   Field.Gender + " varchar(20), " +
                                                   Field.Bio + " varchar(500), " +
                                                   Field.Level + " integer, " +
                                                   Field.Race + " varchar(20), " +
                                                   Field.Class + " varchar(20), " +
                                                   Field.Caster + " boolean, " +
                                                   Field.Hp + " integer, " +
                                                   Field.Constitution + " integer, " +
                                                   Field.Dexterity + " integer, " +
                                                   Field.Strength + " integer, " +
                                                   Field.Charisma + " integer, " +
                                                   Field.Intelligence + " integer, " +
                                                   Field.Wisdom + " integer)";
                
                public const string FindTable = "SELECT COUNT(*) FROM sqlite_master WHERE TYPE='table' AND NAME=@name";
                
                public const string InsertCharacter = "INSERT INTO " + Table + "(" + 
                                                      Field.Name + ", " + 
                                                      Field.Age + ", " + 
                                                      Field.Gender + ", " + 
                                                      Field.Bio + ", " +
                                                      Field.Level + ", " + 
                                                      Field.Race + ", " +
                                                      Field.Class + ", " +
                                                      Field.Caster + ", " +
                                                      Field.Hp + ", " +
                                                      Field.Constitution + ", " +
                                                      Field.Dexterity + ", " +
                                                      Field.Strength + ", " +
                                                      Field.Charisma + ", " +
                                                      Field.Intelligence + ", " +
                                                      Field.Wisdom +
                                                      ") VALUES (@name, @age, @gender, @bio, @level, @race, " +
                                                      "@class, @caster, @hp, @con, @dex, @str, @cha, @intel, @wis)";
                
                public const string FindCharacter = "SELECT COUNT(*) FROM " + Table + 
                                                    " WHERE " + Field.Name + "=@name";
                
                public const string SelectCharacter = "SELECT * FROM " + Table + 
                                                      " WHERE " + Field.Name + "=@name";

                public const string SelectAll = "SELECT * FROM " + Table;
            }

            public static class Field
            {
                public const string Name = "name";
                public const string Age = "age";
                public const string Gender = "gender";
                public const string Bio = "bio";
                public const string Level = "level";
                public const string Race = "race";
                public const string Class = "class";
                public const string Caster = "caster";
                public const string Hp = "hp";
                public const string Constitution = "con";
                public const string Dexterity = "dex";
                public const string Strength = "str";
                public const string Charisma = "cha";
                public const string Intelligence = "int";
                public const string Wisdom = "wis";
            }
        }

        public static class Param
        {
            public const string Name = "@name";
        }
    }
}