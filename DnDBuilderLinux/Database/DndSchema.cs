namespace DnDBuilderLinux.Database
{
    public class DndSchema
    {
        public static class Character
        {
            public const string Table = "characters";
            public const string CreateTableQuery = "create table if not exists " + 
                               Table + "(" +
                               Cols.Name + " varchar(20) primary key, " +
                               Cols.Age + " integer, " +
                               Cols.Gender + " varchar(20), " +
                               Cols.Bio + " varchar(500), " +
                               Cols.Level + " integer, " +
                               Cols.Race + " varchar(20), " +
                               Cols.Class + " varchar(20), " +
                               Cols.Caster + " boolean, " +
                               Cols.Hp + " integer, " +
                               Cols.AbilityScore + " integer)";

            public static class Cols
            {
                public const string Name = "name";
                public const string Age = "age";
                public const string Gender = "gender";
                public const string Bio = "biography";
                public const string Level = "level";
                public const string Race = "race";
                public const string Class = "class";
                public const string Caster = "caster";
                public const string Hp = "hitpoints";
                public const string AbilityScore = "ability";
            }
        }
    }
}