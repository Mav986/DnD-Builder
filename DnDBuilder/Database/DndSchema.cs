namespace DnDBuilder.Database
{
    public class DndSchema
    {
        public static class Character
        {
            public const string Table = "characters";

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